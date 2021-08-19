using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;

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
		private readonly IMapper _mapper;



		public UserService(ICloudBioinformaticsDatabaseSettings settings, IJWTUtils JWTUtils, IMapper mapper)
		{
			MongoClient client = new MongoClient(settings.ConnectionString);
			this.database = client.GetDatabase(settings.DatabaseName);
			this.CollectionName = settings.Users_CollectionName;

			_userEntity = database.GetCollection<UserEntity>(this.CollectionName);

			this._jwtUtils = JWTUtils;
			this._mapper = mapper;
		}

		public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
		{
			IAsyncCursor<UserEntity> userCursor = await _userEntity.FindAsync<UserEntity>(
																		user => user.Username == model.Username);
			UserEntity user = await userCursor.FirstAsync<UserEntity>();

			// Validate
			if (user == null || !BCryptNet.Verify(model.Password, user.PasswordHashed))
			{
				throw new AddException("Username or password is incorrect");
			}

			// Authentication Successful
			AuthenticateResponse response = _mapper.Map<AuthenticateResponse>(user);
			response.JWTToken = this._jwtUtils.GenerateToken(user);
			return response;
		}

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
			if (await this.DoesUserExistsByUsername(model.Username) != null) throw new AddException($"Username '{model.Username}' is already taken!");

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

	}
}