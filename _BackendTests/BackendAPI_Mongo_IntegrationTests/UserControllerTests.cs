using System;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;

using System.Linq;

using Xunit;
using Xunit.Extensions.Ordering;

using Backend.Models;
using Backend.Models.Authentication;
using BackendTests.Utilities;

namespace BackendTests.MongoIntegrationTests
{
	[Collection("MongoDBIntegrationAPI"), Order(1)]
	public class UserControllerTests_Authentication : TestSuiteHelpers, IClassFixture<CustomWebApplicationFactory<Backend.Startup>>
	{
		private readonly CustomWebApplicationFactory<Backend.Startup> _factory;
		private readonly MongoClient _mongoClient;
		private string _refreshTokenCookie;
		private string _accessToken;
		private readonly string _dbName;
		private readonly string _usersCollectionName;
		private readonly string _naturalDNACollectionName;
		private readonly IMongoCollection<UserEntity> _userEntity;
		public UserControllerTests_Authentication(CustomWebApplicationFactory<Backend.Startup> factory)
		{
			this._factory = factory;

			string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";

			this._mongoClient = new(connectionString);
			this._dbName = "cloud_bioinformatics_test";
			this._usersCollectionName = "Users";
			this._userEntity = this._mongoClient.GetDatabase(this._dbName).GetCollection<UserEntity>(this._usersCollectionName);
		}
		/// <summary>
		/// This TC tests, whether we can register a user and log in with her / him.
		/// </summary>
		/// <returns></returns>
		[Fact, Order(1)]
		public async Task TC0001_TestSuiteSetUp_AuthenticationTest()
		{
			// Suite Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			// Arrange
			HttpClient client = _factory.CreateClient();
			HttpRequestMessage requestMessage = new(HttpMethod.Get, "/api/users");
			requestMessage.Headers.Add("Authorization", this._accessToken);
			HttpResponseMessage response = await client.SendAsync(requestMessage);
			response.EnsureSuccessStatusCode(); // Status code 200-299



			// Assert
			string errorMessage = $"Couldn't get Users. - We got StatusCode {response.StatusCode}";
			AssertX.Equal(200, (int)response.StatusCode, errorMessage);
		}
		/// <summary>
		/// This TC tests if we can retrieve a refresh cookie and rotate the refreshToken with it!?
		/// </summary>
		/// <returns></returns>
		[Fact, Order(2)]
		public async Task TC0002_CanRetrieveRefreshTokenAndRotate()
		{
			// TC Setup
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBAuthenticate(_factory);
			HttpClient client = _factory.CreateClient();
			HttpResponseMessage response;

			string errorMessage = $"The refreshCookie seems to be null: '{this._refreshTokenCookie}'";
			AssertX.NotEqual(null, this._refreshTokenCookie, errorMessage);

			// Arrange
			StringContent registerJSONContent = new(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
			registerJSONContent.Headers.Add("Cookie", this._refreshTokenCookie);
			registerJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			response = await client.PostAsync("/api/users/refresh-token", registerJSONContent);
			response.EnsureSuccessStatusCode(); // Status code 200-299
			this._refreshTokenCookie = TestSuiteHelpers.ExtractRefreshTokenFromResponseHeader(response);

			// Assert
			errorMessage = $"The refreshCookie seems to be null: '{this._refreshTokenCookie}'";
			AssertX.NotEqual(null, this._refreshTokenCookie, errorMessage);
		}
		/// <summary>
		/// This TC tests whether we can rotate the refreshTokens without removing them from the DB.
		/// </summary>
		/// <returns></returns>
		[Fact, Order(3)]
		public async Task TC0003_RegisterThenRotateRefreshTokenButNotRemove()
		{
			// TC Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			HttpClient client = _factory.CreateClient();
			HttpResponseMessage response;

			string errorMessage = $"RefreshCookie '{this._refreshTokenCookie}'";
			AssertX.NotEqual(null, this._refreshTokenCookie, errorMessage);

			// Arrange
			response = await RotateRefreshTokenOnce(client, this._refreshTokenCookie);

			// Assert
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
												(_user) => _user.Username == TestSuiteHelpers.registerRequest.Username);
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();

			errorMessage = $"There are more than 2 refreshTokens in the DB: {user.RefreshTokens.Count}";
			AssertX.Equal(2, user.RefreshTokens.Count, errorMessage);
		}
		/// <summary>
		/// This TC tests whether we can rotate the refreshTokens and remove from the DB via revoking one
		/// and setting the TTL back 3 days.
		/// </summary>
		/// <returns></returns>
		[Fact, Order(4)]
		public async Task TC0004_RegisterThenRotateRefreshToken3TimesAndRemoveSecondByRevoke()
		{
			// TC Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			HttpClient client = _factory.CreateClient();
			UserEntity user;


			// Arrange
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			// Revoking a token and setting the creation time back TTL+1 days.
			user = await UpdateUserData();
			// NOTE: We are revoking the second refreshToken, even though it is already revoked, because it gets replaced.
			RefreshToken refreshTokenToModify = user.RefreshTokens[1];
			refreshTokenToModify.Created = DateTime.UtcNow.AddDays(-3);
			refreshTokenToModify.Revoked = DateTime.UtcNow;
			await UpdateRefreshToken(user, refreshTokenToModify);


			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			user = await UpdateUserData();
			// Assert
			string errorMessage = $"There are other than 3 refreshTokens in the DB: {user.RefreshTokens.Count}";
			AssertX.Equal(3, user.RefreshTokens.Count, errorMessage);
			// throw new System.NotImplementedException();
		}
		/// <summary>
		/// This TC tests, whether we can rotate the refreshTokens and remove one via setting the TTL and the Expiry back.
		/// Beyond the threshold.
		/// </summary>
		/// <returns></returns>
		[Fact, Order(5)]
		public async Task TC0005_RegisterThenRotateRefreshToken3TimesAndRemoveSecondByExpiry()
		{
			// TC Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			HttpClient client = _factory.CreateClient();
			UserEntity user;


			// Arrange
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			// Revoking a token and setting the creation time back TTL+1 days.
			user = await UpdateUserData();
			RefreshToken refreshTokenToModify = user.RefreshTokens[1];
			refreshTokenToModify.Created = DateTime.UtcNow.AddDays(-3);
			refreshTokenToModify.Expires = DateTime.UtcNow.AddDays(-8);
			await UpdateRefreshToken(user, refreshTokenToModify);


			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			user = await UpdateUserData();
			// Assert
			string errorMessage = $"There are other than 3 refreshTokens in the DB: {user.RefreshTokens.Count}";
			AssertX.Equal(3, user.RefreshTokens.Count, errorMessage);
			// throw new System.NotImplementedException();
		}
		/// <summary>
		/// This TC tests, whether the refreshToken we want to remove gets removed via TTL, Expiry and Revoking.
		/// </summary>
		/// <returns></returns>
		[Fact, Order(6)]
		public async Task TC0006_RegisterThenRotateRefreshToken3TimesAndRemoveSecondByExpiryANDRevoking()
		{
			// TC Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			HttpClient client = _factory.CreateClient();
			UserEntity user;


			// Arrange
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			// Revoking a token and setting the creation time back TTL+1 days.
			user = await UpdateUserData();
			RefreshToken refreshTokenToModify = user.RefreshTokens[1];
			refreshTokenToModify.Created = DateTime.UtcNow.AddDays(-3);
			refreshTokenToModify.Expires = DateTime.UtcNow.AddDays(-8);
			refreshTokenToModify.Revoked = DateTime.UtcNow;
			await UpdateRefreshToken(user, refreshTokenToModify);


			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			user = await UpdateUserData();
			// Assert
			string errorMessage = $"There are other than 3 refreshTokens in the DB: {user.RefreshTokens.Count}";
			AssertX.Equal(3, user.RefreshTokens.Count, errorMessage);
			// throw new System.NotImplementedException();
		}
		/// <summary>
		/// When a refreshToken is used more than once, being reused for acquiring a new JWT accessToken,
		/// all the active JWTs should go to being BlackListed.
		/// 
		/// <para> This should mean, that access is restricted for the attacker, when User tries to
		/// renew her/his JWT accessToken.</para>
		/// </summary>
		/// <returns></returns>
		[Fact, Order(7)]
		public async Task TC0007_RevokeRefreshTokenByReuseOfAncestor_TestAcceptanceOfJWTAccessToken_and_RefreshToken()
		{
			// TC Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			(this._refreshTokenCookie, this._accessToken) = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

			HttpClient client = _factory.CreateClient();

			// Arrange
			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			string ancestralAccessToken = this._accessToken;
			string ancestralRefreshCookie = this._refreshTokenCookie;

			await RotateRefreshTokenOnce(client, this._refreshTokenCookie);
			await RotateRefreshTokenOnce(client, ancestralRefreshCookie, true);

			HttpRequestMessage requestMessage = new(HttpMethod.Get, "/api/users");
			requestMessage.Headers.Add("Authorization", ancestralAccessToken);
			HttpResponseMessage response = await client.SendAsync(requestMessage);
			string errorMessage = "The server DID accept a BlackListed JWT token!";
			AssertX.NotEqual(200, (int)response.StatusCode, errorMessage); // Status code is NOT 200;

			// Rotate refresh token
			StringContent registerJSONContent = new(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
			registerJSONContent.Headers.Add("Cookie", this._refreshTokenCookie);
			registerJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			response = await client.PostAsync("/api/users/refresh-token", registerJSONContent);

			errorMessage = "The server DID accept a revoked refreshToken!";
			AssertX.NotEqual(200, (int)response.StatusCode, errorMessage); // Status code is NOT 200;
		}
		/// <summary>
		/// This helper method Rotates the refreshToken, using the UserServices. Minor issue is that 
		/// it uses the this._refreshCookie variable available from the class.
		/// </summary>
		/// <param name="client"></param>
		/// <returns>Returns the response from the refresh-token endpoint.</returns>
		private async Task<HttpResponseMessage> RotateRefreshTokenOnce(HttpClient client, string refreshCookie, bool errorProne = false)
		{
			StringContent registerJSONContent = new(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
			registerJSONContent.Headers.Add("Cookie", refreshCookie);
			registerJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			HttpResponseMessage response = await client.PostAsync("/api/users/refresh-token", registerJSONContent);

			// If error prone, that means we might not get back the Cookies or a proper HTTP200 response.
			if (!errorProne)
			{
				response.EnsureSuccessStatusCode(); // Status code 200-299
				this._accessToken = TestSuiteHelpers.ExtractAccessTokenFromResponseBody(response);
				this._refreshTokenCookie = TestSuiteHelpers.ExtractRefreshTokenFromResponseHeader(response);
			}

			return response;
		}
		private async Task UpdateRefreshToken(UserEntity user, RefreshToken token)
		{
			FilterDefinition<UserEntity> filter = Builders<UserEntity>.Filter.And(
																Builders<UserEntity>.Filter.Eq((_user) => _user.Id, user.Id),
																Builders<UserEntity>.Filter.ElemMatch((_user) => _user.RefreshTokens,
																										(_field) => _field.Token == token.Token));
			UpdateDefinition<UserEntity> update = Builders<UserEntity>.Update.Set((_user) => _user.RefreshTokens[-1], token);
			await _userEntity.UpdateOneAsync(filter, update);

			return;
		}
		private async Task<UserEntity> UpdateUserData()
		{
			IAsyncCursor<UserEntity> requestResults = await _userEntity.FindAsync<UserEntity>(
															(_user) => _user.Username == TestSuiteHelpers.registerRequest.Username);
			UserEntity user = await requestResults.FirstOrDefaultAsync<UserEntity>();

			return user;
		}
	}
}