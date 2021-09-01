using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Backend.Models;
using Backend.Models.UserManagement;

namespace BackendTests.Utilities
{
	public interface TestSuiteHelpers
	{
		private static readonly RegisterRequest registerRequest = new()
		{
			Username = "vertex",
			Password = "#33FalleN666#",
			RePassword = "33FalleN666"
		};
		private static readonly AuthenticateRequest authenticateRequest = new()
		{
			Username = "vertex",
			Password = "#33FalleN666#"
		};

		public static async Task<string> MongoDBRegisterAndAuthenticate(CustomWebApplicationFactory<Backend.Startup> _factory)
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
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);
			string accessToken = (string)jsonResponse["AccessToken"];

			return accessToken;
		}
		public static async Task<string> MongoDBAuthenticate(CustomWebApplicationFactory<Backend.Startup> _factory)
		{
			HttpClient client = _factory.CreateClient();

			// Authenticate
			StringContent authenticateJSONContent = new(JsonConvert.SerializeObject(authenticateRequest), Encoding.UTF8, "application/json");
			authenticateJSONContent.Headers.Add("X-Forwarded-For", "127.0.0.1");
			HttpResponseMessage response = await client.PostAsync("/api/users/authenticate", authenticateJSONContent);
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);
			string accessToken = (string)jsonResponse["AccessToken"];

			return accessToken;
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
	}
}