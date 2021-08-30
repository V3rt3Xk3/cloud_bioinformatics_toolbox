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

		// public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
		// {

		// }

		public async Task<List<UserEntity>> GetAsync()
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync(NaturalDNASequence => true);
			return await requestResults.ToListAsync();
		}

		public async Task<UserEntity> GetAsyncById(string id)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
																						user => user.Id == id);
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();
			if (user == null) return null;

			return user;
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

		// NOTE: Helper functions:
		// BUG: I might want to rewrite it to check whether result is null.
		private async Task<UserEntity> DoesUserExistsByUsername(string username)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
																			user => user.Username == username);
			return await requestResults.FirstOrDefaultAsync<UserEntity>();
		}

		private async Task<UserEntity> getUserByRefreshToken(string token)
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
												(_user) => _user.RefreshTokens.Any((_token) => _token.Token == token));
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();
			if (user == null) throw new AppException("Invalid token");

			return user;
		}

		// private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
		// {
		// 	RefreshToken newToken = _jwtUtils.GenerateRefreshToken(ipAddress);

		// }

		private async Task RemoveOldRefreshTokens(UserEntity user)
		{
			// Remove old inactive refresh tokens from user based on TTL in app settings
			// FIXME: This update filter checks for an AND relationship betwen active and obsolete refresh tokens
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.PullFilter((_document) => _document.RefreshTokens,
					(_field) => !_field.IsActive && (_field.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow));

			await _userEntity.UpdateOneAsync((_user) => _user.Id == user.Id, update, new UpdateOptions() { IsUpsert = true });
			return;
		}
	}
}