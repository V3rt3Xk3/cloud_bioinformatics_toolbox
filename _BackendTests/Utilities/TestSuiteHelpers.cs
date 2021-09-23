using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Backend.Models;
using Backend.Models.UserManagement;

namespace BackendTests.Utilities
{
	public class TestSuiteHelpers
	{
		public static readonly RegisterRequest registerRequest = new()
		{
			Email = "vertex@vertex.hu",
			Password = "#33FalleN666#",
			RePassword = "33FalleN666"
		};
		public static readonly AuthenticateRequest authenticateRequest = new()
		{
			Email = "vertex@vertex.hu",
			Password = "#33FalleN666#"
		};

		public static async Task<(string, string)> MongoDBRegisterAndAuthenticate(CustomWebApplicationFactory<Backend.Startup> _factory)
		{
			// Arrange
			HttpClient client = _factory.CreateClient();
			HttpResponseMessage response;

			// Register
			StringContent registerJSONContent = new(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");
			registerJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			response = await client.PostAsync("/api/users/register", registerJSONContent);
			response.EnsureSuccessStatusCode(); // Status code 200-299

			// Authenticate
			StringContent authenticateJSONContent = new(JsonConvert.SerializeObject(authenticateRequest), Encoding.UTF8, "application/json");
			authenticateJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			response = await client.PostAsync("/api/users/authenticate", authenticateJSONContent);
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string accessToken = ExtractAccessTokenFromResponseBody(response);
			string refreshToken = ExtractRefreshTokenFromResponseHeader(response);

			return (refreshToken, accessToken);
		}
		public static async Task<(string, string)> MongoDBAuthenticate(CustomWebApplicationFactory<Backend.Startup> _factory)
		{

			HttpClient client = _factory.CreateClient();

			// Authenticate
			StringContent authenticateJSONContent = new(JsonConvert.SerializeObject(authenticateRequest), Encoding.UTF8, "application/json");
			authenticateJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			HttpResponseMessage response = await client.PostAsync("/api/users/authenticate", authenticateJSONContent);
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string accessToken = ExtractAccessTokenFromResponseBody(response);

			string refreshToken = ExtractRefreshTokenFromResponseHeader(response);


			return (refreshToken, accessToken);
		}

		/// <summary>
		/// Cleans up the MongoDB "cloud_bioinformatics_test" test database. | Drops it.
		/// </summary>
		public static void MongoDBCleanUp(MongoClient _client, string _dbName = "cloud_bioinformatics_test")
		{
			_client.DropDatabase(_dbName);
		}
		public static void MongoDBCollectionCleanup<TClass>(MongoClient _client, string _collectionName, string _dbName = "cloud_bioinformatics_test")
		{
			IMongoDatabase dataBase = _client.GetDatabase(_dbName);
			IMongoCollection<TClass> userCollection = dataBase.GetCollection<TClass>(_collectionName);

			userCollection.DeleteMany(Builders<TClass>.Filter.Empty);
		}
		public static string ExtractRefreshTokenFromResponseHeader(HttpResponseMessage response)
		{
			System.Net.Http.Headers.HttpResponseHeaders responseHeaders = response.Headers;

			string[] settedCookies = (string[])responseHeaders.GetValues("set-cookie");

			string refreshTokenCookie = null;
			int i = 0;
			while (i < settedCookies.Length)
			{
				if (settedCookies[i].Split(';')[0].Split('=')[0] == "refreshToken")
				{
					refreshTokenCookie += settedCookies[i].Split(';')[0];
				}
				i++;
			}

			return refreshTokenCookie;
		}
		public static string ExtractAccessTokenFromResponseBody(HttpResponseMessage response)
		{
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);
			string accessToken = (string)jsonResponse["AccessToken"];

			return accessToken;
		}
	}
}