using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;

using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

using Backend.Authorization;
using Backend.Models;
using Backend.Models.Authentication;
using Backend.Models.UserManagement;
using Backend.Helpers;

namespace Backend.Services
{
	public class UserService : IUserService
	{
		private readonly IMongoCollection<UserEntity> _userEntity;
		private readonly IMongoDatabase database;
		public string CollectionName { get; }
		private readonly IJWTUtils _jwtUtils;
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
			if (await this.DoesUserExistsByUsername(model.Username)) throw new AppException($"Username '{model.Username}' is already taken!");

			// RegisterRequest mapped to UserEntity
			UserEntity user = _mapper.Map<UserEntity>(model);
			user.TotalJWTBlackListCount = 0;

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


			(string JWTAccessToken, string accessTokenID) = this._jwtUtils.GenerateAccessToken(user);

			RefreshToken refreshToken = this._jwtUtils.GenerateRefreshToken(ipAddress, accessTokenID);

			// Authentication Successful
			AuthenticateResponse authResponse = new(user, JWTAccessToken, refreshToken.Token);

			// WOW: This looked like the more verbose way to do filtering. 
			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.Eq("Id", user.Id);
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.AddToSet("RefreshTokens", refreshToken);
			_userEntity.UpdateOne(filter, update);


			return authResponse;
		}

		public async Task<AuthenticateResponse> RefreshToken(string refreshTokenStringRepresentation, string ipAddress)
		{
			UserEntity user = await GetUserByRefreshToken(refreshTokenStringRepresentation);
			RefreshToken refreshToken = user.RefreshTokens.Single((_token) => _token.Token == refreshTokenStringRepresentation);
			RefreshTokenRevokationSettings revokeSettings;

			if (IsTokenRevoked(refreshToken))
			{
				// Revoke all descendant tokens in case of this token has been compormised
				revokeSettings = new RefreshTokenRevokationSettings(refreshToken, ipAddress, refreshToken.Token, true);
				await BlackListJWTFromRefreshToken(user, revokeSettings);
				await RevokeDescendantRefreshTokens(revokeSettings, user, $"Attempted reuse of revoked ancestor token: {refreshTokenStringRepresentation}");
			}

			if (!IsTokenActive(refreshToken)) throw new AppException("Invalid token");

			// Generate new jwt accessToken
			(string accessToken, string accessTokenID) = _jwtUtils.GenerateAccessToken(user);

			// Replace old refresh token with a new one (rotate token)
			revokeSettings = new RefreshTokenRevokationSettings(refreshToken, ipAddress, null, false);
			RefreshToken newRefreshToken = await RotateRefreshToken(user, revokeSettings, accessTokenID);

			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id);
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.Push((_user) => _user.RefreshTokens, newRefreshToken);
			await _userEntity.UpdateOneAsync(filter, update);

			// Remove old tokens
			await RemoveOldRefreshTokens(user);

			return new AuthenticateResponse(user, accessToken, newRefreshToken.Token);
		}

		public async Task RevokeToken(string token, string ipAddress)
		{
			UserEntity user = await GetUserByRefreshToken(token);
			RefreshToken refreshToken = user.RefreshTokens.SingleOrDefault((_token) => _token.Token == token);

			if (!IsTokenActive(refreshToken)) throw new AppException("Invalid token");

			// Revoke token and save
			// NOTE: Again, for me it seems like the DB access is done in the private method.
			RefreshTokenRevokationSettings revokeSettings = new(refreshToken, ipAddress);
			await RevokeRefreshToken(user, revokeSettings, "Revoked without replacement!");
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
		/// <summary>
		/// This function returns True if user exists and False if not.
		/// <para>Queries the username against the DB.</para>
		/// </summary>
		/// <param name="username">String -> Queryable userName against the DB.</param>
		/// <returns>True if exists | False if not</returns>
		private async Task<bool> DoesUserExistsByUsername(string username)
		{
			IFindFluent<UserEntity, UserEntity> requestResults = _userEntity.Find<UserEntity>((_user) => _user.Username == username).Limit(1);
			return (await requestResults.FirstOrDefaultAsync<UserEntity>()) != null;
		}
		/// <summary>
		/// This method is responsible to acces the userId corresponding to a given refreshToken
		/// </summary>
		/// <param name="token"></param>
		/// <returns>[UserEntity] user</returns>
		private async Task<UserEntity> GetUserByRefreshToken(string token)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
												(_user) => _user.RefreshTokens.Any((_token) => _token.Token == token));
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();
			if (user == null) throw new AppException("Invalid token");

			return user;
		}
		/// <summary>
		/// This method rotates a refresh token. Practically issues a new refreshToken and a new JWT AccessToken
		/// </summary>
		/// <param name="user"></param>
		/// <param name="revokeSettings"></param>
		/// <param name="issuedJWT"></param>
		/// <returns>[RefreshToken] Returns a new refreshToken</returns>
		private async Task<RefreshToken> RotateRefreshToken(UserEntity user, RefreshTokenRevokationSettings revokeSettings, string issuedJWT)
		{
			RefreshToken newToken = _jwtUtils.GenerateRefreshToken(revokeSettings.IpAddress, issuedJWT);

			await RevokeRefreshToken(user, revokeSettings, "Replaced by new token!", newToken.Token);
			return newToken;
		}
		/// <summary>
		/// Removes old refresh tokens from the given user DB entry.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private async Task RemoveOldRefreshTokens(UserEntity user)
		{
			// Remove old inactive refresh tokens from user based on TTL in app settings
			// FIXME: This update filter checks for an AND relationship betwen active and obsolete refresh tokens
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.PullFilter((_document) => _document.RefreshTokens,
					(_field) => ((_field.Revoked != null) ||
								(_field.Expires <= DateTime.UtcNow)) &&
								(_field.Created <= DateTime.UtcNow.AddDays(-1 * _appSettings.RefreshTokenTTL)));

			await _userEntity.UpdateOneAsync((_user) => _user.Id == user.Id, update, new UpdateOptions() { IsUpsert = true });
			return;
		}
		/// <summary>
		/// Revokes the descendant refreshTokens from an origin.
		/// <para>Be aware, that it is a recursive function.</para>
		/// </summary>
		/// <param name="revokeSettings"></param>
		/// <param name="user"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		private async Task RevokeDescendantRefreshTokens(RefreshTokenRevokationSettings revokeSettings,
															UserEntity user,
															string reason)
		{
			RefreshToken refreshToken = revokeSettings.RefreshTokenToRemove;
			// FIXME: This method needs testing. - mark if DONE
			// recursively traverse the refresh token chain and ensure all descendants are revoked
			if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
			{
				RefreshToken childToken = user.RefreshTokens.SingleOrDefault((_token) => _token.Token == refreshToken.ReplacedByToken);
				revokeSettings.RefreshTokenToRemove = childToken;
				if (IsTokenActive(childToken)) await RevokeRefreshToken(user,
																		revokeSettings,
																		reason);
				else
				{
					await BlackListJWTFromRefreshToken(user, revokeSettings);
					await RevokeDescendantRefreshTokens(revokeSettings, user, reason);
				}
			}
		}
		// TODO: This could be a point of optimization as we make a DB entry everytime a token is modified.
		/// <summary>
		/// This method revokes a refresh token and blacklists the corresponding JWT, if there is 
		/// a possibility to refreshToken theft. 
		/// <para>The refreshToken breach possibility is stored in the revokeSettings struct</para>
		/// </summary>
		/// <param name="user"></param>
		/// <param name="revokeSettings"></param>
		/// <param name="reason"></param>
		/// <param name="replacedByToken"></param>
		/// <returns></returns>
		private async Task RevokeRefreshToken(UserEntity user,
												RefreshTokenRevokationSettings revokeSettings,
												string reason = null,
												string replacedByToken = null)
		{
			RefreshToken refreshToken = revokeSettings.RefreshTokenToRemove;

			FilterDefinition<UserEntity> filter;
			UpdateDefinition<UserEntity> update;

			refreshToken.Revoked = DateTime.UtcNow;
			refreshToken.RevokedByIp = revokeSettings.IpAddress;
			refreshToken.ReasonsRevoked = reason;
			refreshToken.ReplacedByToken = replacedByToken;

			// BUG: This DB query needs explicit testing. - mark if done!
			filter = Builders<UserEntity>.Filter.And(
													Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id),
													Builders<UserEntity>.Filter.ElemMatch((_user) => _user.RefreshTokens,
																							(_field) => _field.Token == refreshToken.Token));
			update = Builders<UserEntity>.Update.Set((_user) => _user.RefreshTokens[-1], refreshToken);
			await _userEntity.UpdateOneAsync(filter, update);

			if (revokeSettings.PossibleRefreshTokenTheft)
			{
				await BlackListJWTFromRefreshToken(user, revokeSettings);
			}

			return;
		}

		private static bool IsTokenActive(RefreshToken token) => (!IsTokenRevoked(token) && !IsTokenExpired(token));

		private static bool IsTokenRevoked(RefreshToken token) => token.Revoked != null;
		private static bool IsTokenExpired(RefreshToken token) => (DateTime.UtcNow >= token.Expires);
		/// <summary>
		/// Blacklists a JWT corresponding to the given **revokeSettings.RefreshToken**.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="revokeSettings"></param>
		/// <returns></returns>
		private async Task BlackListJWTFromRefreshToken(UserEntity user, RefreshTokenRevokationSettings revokeSettings)
		{
			RefreshToken refreshToken = revokeSettings.RefreshTokenToRemove;
			string ipAddress = revokeSettings.IpAddress;

			user = await GetUserByIdAsync(user.Id);
			FilterDefinition<UserEntity> filter;
			UpdateDefinition<UserEntity> update;

			int blackListedJWTCount = user.TotalJWTBlackListCount + 1;

			IEnumerable<BlackListedJWT> BlackListMatches;
			BlackListedJWT newBlackListedJWT;
			bool isItPush = true;
			if (user.BlackListedJWTs != null)
			{
				BlackListMatches = user.BlackListedJWTs.Where((_token) => _token.CorrespondingRefreshToken == refreshToken.Token);
				if (!BlackListMatches.Any())
				{
					newBlackListedJWT = new(refreshToken.IssuedJWTTokenId, revokeSettings.FirstAncestor, refreshToken.Created, ipAddress, refreshToken.Token);
				}
				else
				{
					isItPush = false;
					newBlackListedJWT = BlackListMatches.First();
					newBlackListedJWT.AttemptsToReuse += 1;
				}
			}
			else
			{
				newBlackListedJWT = new(refreshToken.IssuedJWTTokenId, revokeSettings.FirstAncestor, refreshToken.Created, ipAddress, refreshToken.Token);
			}

			if (isItPush)
			{
				filter = Builders<UserEntity>.Filter.Eq("Id", user.Id);
				update = Builders<UserEntity>.Update.AddToSet("BlackListedJWTs", newBlackListedJWT)
													.Set("TotalJWTBlackListCount", blackListedJWTCount);
				_userEntity.UpdateOne(filter, update);
			}
			else
			{
				filter = Builders<UserEntity>.Filter.Eq("Id", user.Id);
				update = Builders<UserEntity>.Update.Set("TotalJWTBlackListCount", blackListedJWTCount);
				_userEntity.UpdateOne(filter, update);

				filter = Builders<UserEntity>.Filter.And(Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id),
												Builders<UserEntity>.Filter.ElemMatch((_user) => _user.BlackListedJWTs,
																					(_field) => _field.CorrespondingRefreshToken == refreshToken.Token));
				update = Builders<UserEntity>.Update.Set((_user) => _user.BlackListedJWTs[-1], newBlackListedJWT);
				_userEntity.UpdateOne(filter, update);
			}


			// This is an explicit Sync function, because it is high priority.

		}
	}
	/// <summary>
	/// This is a struct that contains most of the settings needed for revoking a [RefreshToken]
	/// </summary>
	public struct RefreshTokenRevokationSettings
	{
		public RefreshToken RefreshTokenToRemove { get; set; }
		public string FirstAncestor { get; }
		public string IpAddress { get; }
		public bool PossibleRefreshTokenTheft { get; }

		public RefreshTokenRevokationSettings(RefreshToken refreshToken, string ipAddress, string firstAncestor = null, bool possibleRefreshTokenTheft = false)
		{
			this.RefreshTokenToRemove = refreshToken;
			this.FirstAncestor = firstAncestor;
			this.IpAddress = ipAddress;
			this.PossibleRefreshTokenTheft = possibleRefreshTokenTheft;
		}
	}
}