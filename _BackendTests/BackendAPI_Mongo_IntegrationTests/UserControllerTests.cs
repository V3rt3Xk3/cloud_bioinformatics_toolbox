using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;

using Xunit;
using Xunit.Extensions.Ordering;

using Backend.Models;
using Backend.Models.UserManagement;
using BackendTests.Utilities;

namespace BackendTests.MongoIntegrationTests
{
	[Collection("MongoDBIntegrationAPI"), Order(1)]
	public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Backend.Startup>>, TestSuiteHelpers
	{
		private readonly CustomWebApplicationFactory<Backend.Startup> _factory;
		private MongoClient _mongoClient;
		private string _accessToken;
		private readonly string _dbName;
		private readonly string _usersCollectionName;
		private readonly string _naturalDNACollectionName;
		public UserControllerTests(CustomWebApplicationFactory<Backend.Startup> factory)
		{
			this._factory = factory;

			string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";

			this._mongoClient = new(connectionString);
			this._dbName = "cloud_bioinformatics_test";
			this._usersCollectionName = "Users";
			// this._naturalDNACollectionName = "NaturalDNASequences";
		}

		[Fact, Order(1)]
		public async Task TC0001_TestSuiteSetUp()
		{
			// Suite Setup
			TestSuiteHelpers.MongoDBCleanUp(this._mongoClient);
			this._accessToken = await TestSuiteHelpers.MongoDBRegisterAndAuthenticate(this._factory);

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
	}
}