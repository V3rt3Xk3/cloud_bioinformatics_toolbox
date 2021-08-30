using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;

using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

using Backend.Authorization;
using Backend.Models;
using Backend.Models.UserManagement;
using Backend.Helpers;

namespace Backend.Services
{
	public class UserService : IUserService
	{
		private readonly IMongoCollection<UserEntity> _userEntity;
		private readonly IMongoDatabase database;
		public string CollectionName { get; }
		private IJWTUtils _jwtUtils;
		private readonly AppSettings _appSettings;
		private readonly IMapper _mapper;



		public UserService(ICloudBioinformaticsDatabaseSettings settings, IJWTUtils JWTUtils, IMapper mapper, IOptions<AppSettings> appSettings)
		{
			MongoClient client = new(settings.ConnectionString);
			this.database = client.GetDatabase(settings.DatabaseName);
			this.CollectionName = settings.Users_CollectionName;

			_userEntity = database.GetCollection<UserEntity>(this.CollectionName);

			this._appSettings = appSettings.Value;
			this._jwtUtils = JWTUtils;
			this._mapper = mapper;
		}

		public async Task Register(RegisterRequest model)
		{
			// Validate
			if (await this.DoesUserExistsByUsername(model.Username) != null) throw new AppException($"Username '{model.Username}' is already taken!");

			// RegisterRequest mapped to UserEntity
			UserEntity user = _mapper.Map<UserEntity>(model);

			// hashing Password
			// BUG: we don't test whether the Password and RePassword match. IT SHOULD BE DONE.
			user.PasswordHashed = BCryptNet.HashPassword(model.Password);

			// Save User
			_userEntity.InsertOne(user);
			return;
		}

		public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
		{
			IAsyncCursor<UserEntity> userCursor = await _userEntity.FindAsync<UserEntity>(
																		user => user.Username == model.Username);
			UserEntity user = await userCursor.FirstOrDefaultAsync<UserEntity>();

			// Validate
			if (user == null || !BCryptNet.Verify(model.Password, user.PasswordHashed))
			{
				throw new AppException("Username or password is incorrect");
			}


			string JWTToken = this._jwtUtils.GenerateAccessToken(user);

			RefreshToken refreshToken = this._jwtUtils.GenerateRefreshToken(ipAddress);

			// Authentication Successful
			AuthenticateResponse authResponse = new(user, JWTToken, refreshToken.Token);

			// WOW: This looked like the only 
			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.Eq("Id", user.Id);
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.AddToSet("RefreshTokens", refreshToken);
			_userEntity.UpdateOne(filter, update);


			return authResponse;
		}

		public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
		{
			UserEntity user = await GetUserByRefreshToken(token);
			RefreshToken refreshToken = user.RefreshTokens.Single((_token) => _token.Token == token);

			if (IsTokenRevoked(refreshToken))
			{
				// Revoke all descendant tokens in case of this token has been compormised
				await RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
				// NOTE: At this point for me, it seems like the revokeRefreshToken updates the DB.
				// Refer to: https://jasonwatmore.com/post/2021/06/15/net-5-api-jwt-authentication-with-refresh-tokens > UserServices.cs
			}

			if (!IsTokenActive(refreshToken)) throw new AppException("Invalid token");

			// Replace old refresh token with a new one (rotate token)
			RefreshToken newRefreshToken = await RotateRefreshToken(user, refreshToken, ipAddress);

			// FIXME: Again this DB entry needs testing as well - mark if done.
			// FIXME: Might as well do a refactoring.
			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id);
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.Push((_user) => _user.RefreshTokens, newRefreshToken);
			await _userEntity.UpdateOneAsync(filter, update);

			// Remove old tokens
			await RemoveOldRefreshTokens(user);

			// Generate new jwt accessToken
			string accessToken = _jwtUtils.GenerateAccessToken(user);

			return new AuthenticateResponse(user, accessToken, newRefreshToken.Token);
		}

		public async Task RevokeToken(string token, string ipAddress)
		{
			UserEntity user = await GetUserByRefreshToken(token);
			RefreshToken refreshToken = user.RefreshTokens.SingleOrDefault((_token) => _token.Token == token);

			if (!IsTokenActive(refreshToken)) throw new AppException("Invalid token");

			// Revoke token and save
			await RevokeRefreshToken(user, refreshToken, ipAddress, "Revoked without replacement!");
			// NOTE: Again, for me it seems like the DB access is done in the private method.
			return;
		}

		public async Task<List<UserEntity>> GetAllUsersAsync()
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync(NaturalDNASequence => true);
			return await requestResults.ToListAsync();
		}

		public async Task<UserEntity> GetUserByIdAsync(string id)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
																						user => user.Id == id);
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();
			if (user == null) return null;

			return user;
		}

		// NOTE: Helper functions:
		// BUG: I might want to rewrite it to check whether result is null.
		private async Task<UserEntity> DoesUserExistsByUsername(string username)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
																			user => user.Username == username);
			return await requestResults.FirstOrDefaultAsync<UserEntity>();
		}

		private async Task<UserEntity> GetUserByRefreshToken(string token)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
												(_user) => _user.RefreshTokens.Any((_token) => _token.Token == token));
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();
			if (user == null) throw new AppException("Invalid token");

			return user;
		}

		private async Task<RefreshToken> RotateRefreshToken(UserEntity user, RefreshToken oldRefreshToken, string ipAddress)
		{
			RefreshToken newToken = _jwtUtils.GenerateRefreshToken(ipAddress);
			await RevokeRefreshToken(user, oldRefreshToken, ipAddress, "Replaced by new token!", newToken.Token);
			return newToken;
		}

		private async Task RemoveOldRefreshTokens(UserEntity user)
		{
			// Remove old inactive refresh tokens from user based on TTL in app settings
			// FIXME: This update filter checks for an AND relationship betwen active and obsolete refresh tokens
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.PullFilter((_document) => _document.RefreshTokens,
					(_field) => (_field.Revoked != null) &&
								(_field.Expires <= DateTime.UtcNow));

			await _userEntity.UpdateOneAsync((_user) => _user.Id == user.Id, update, new UpdateOptions() { IsUpsert = true });
			return;
		}

		private async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, UserEntity user, string ipAddress, string reason)
		{
			// FIXME: This method needs testing.
			// recursively traverse the refresh token chain and ensure all descendants are revoked
			if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
			{
				RefreshToken childToken = user.RefreshTokens.SingleOrDefault((_token) => _token.Token == refreshToken.ReplacedByToken);
				if (IsTokenActive(childToken)) await RevokeRefreshToken(user, childToken, ipAddress, reason);
				else await RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);

			}
		}
		// TODO: This could be a point of optimization as we make a DB entry everytime a token is modified.
		private async Task RevokeRefreshToken(UserEntity user, RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
		{
			token.Revoked = DateTime.UtcNow;
			token.RevokedByIp = ipAddress;
			token.ReasonsRevoked = reason;
			token.ReplacedByToken = replacedByToken;

			// BUG: This DB query needs explicit testing. - mark if done!
			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.And(
													Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id),
													Builders<UserEntity>.Filter.ElemMatch((_user) => _user.RefreshTokens,
																							(_field) => _field.Token == token.Token));
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.Set((_user) => _user.RefreshTokens[-1], token);
			await _userEntity.UpdateOneAsync(filter, update);
			return;
		}

		private bool IsTokenActive(RefreshToken token) => (!IsTokenRevoked(token) && !IsTokenExpired(token));

		private bool IsTokenRevoked(RefreshToken token) => token.Revoked != null;
		private bool IsTokenExpired(RefreshToken token) => (DateTime.UtcNow >= token.Expires);
	}
}