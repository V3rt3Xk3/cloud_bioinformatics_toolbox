using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;

using Xunit;
using Xunit.Extensions.Ordering;

using Backend.Models;
using BackendTests.Utilities;

namespace BackendTests
{
	public class NaturalDNAControllerTests : IClassFixture<CustomWebApplicationFactory<Backend.Startup>>, ITestSuite
	{
		private readonly CustomWebApplicationFactory<Backend.Startup> _factory;
		private string _accessToken;
		private MongoClient _mongoClient;
		private readonly string _dbName;
		private readonly string _usersCollectionName;
		private readonly string _naturalDNACollectionName;
		public NaturalDNAControllerTests(CustomWebApplicationFactory<Backend.Startup> factory)
		{
			this._factory = factory;

			string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";
			this._mongoClient = new(connectionString);

			this._dbName = "cloud_bioinformatics_test";
			this._usersCollectionName = "Users";
			this._naturalDNACollectionName = "NaturalDNASequences";

		}

		[Fact, Order(1)]
		public async Task TC0001_TestSuiteSetUp()
		{
			ITestSuite.MongoDBCleanUp(_mongoClient);
			this._accessToken = await ITestSuite.MongoDBRegisterAndAuthenticate(_factory);
			// Arrange
			HttpClient client = _factory.CreateClient();

			// Act
			HttpRequestMessage requestMessage = new(HttpMethod.Get, "/api/naturalDNA");
			requestMessage.Headers.Add("Authorization", this._accessToken);
			HttpResponseMessage response = await client.SendAsync(requestMessage);
			string responseString = response.Content.ReadAsStringAsync().Result;
			JArray jsonArray = JArray.Parse(responseString);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string errorMessage = "There are some entries in the DB - \"NaturalDNASequences\" collection.";
			AssertX.Equal(0, jsonArray.Count, errorMessage);
		}

		[Theory, Order(2)]
		[InlineData("/api/naturalDNA")]
		public async Task TC0002_CheckingForEndpoint_StatusCodes_ContentType(string url)
		{
			// Arrange
			HttpClient client = _factory.CreateClient();
			this._accessToken = await ITestSuite.MongoDBAuthenticate(_factory);

			// Act
			HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
			requestMessage.Headers.Add("Authorization", this._accessToken);
			HttpResponseMessage response = await client.SendAsync(requestMessage);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string errorMessage = "The repsonse content is not: 'application/json; charset=utf-8'";
			AssertX.Equal("application/json; charset=utf-8",
							response.Content.Headers.ContentType.ToString(),
							errorMessage);
		}

		[Fact, Order(3)]
		public async Task TC0003_CheckingFor_InsertingOne_ReceivingTheInput()
		{
			// Arrange
			HttpClient client = _factory.CreateClient();
			this._accessToken = await ITestSuite.MongoDBAuthenticate(_factory);

			string json2POST = NaturalDNA_TestUtilities.LoadTestData_SingleEntity();
			StringContent jsonContent = new(json2POST, Encoding.UTF8, "application/json");
			HttpRequestMessage requestMessage = new(HttpMethod.Post, "/api/naturalDNA");
			requestMessage.Content = jsonContent;
			requestMessage.Headers.Add("Authorization", this._accessToken);

			// Act
			HttpResponseMessage response = await client.SendAsync(requestMessage);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);

			string errorMessage = "The inserted Sequence document had a different \"sequenceName\" than \"Test 1\"";
			AssertX.Equal("Sus scrofa breed Landrace oxytocin gene", jsonResponse["sequenceName"], errorMessage);

			ITestSuite.MongoDBCollectionCleanup<NaturalDNASequenceEntity>(_mongoClient, _naturalDNACollectionName);
		}
		[Fact, Order(4)]
		public async Task TC0004_CheckingFor_MultipleInsertingOne()
		{
			// Arrange
			HttpClient client = _factory.CreateClient();
			this._accessToken = await ITestSuite.MongoDBAuthenticate(_factory);
			HttpRequestMessage requestMessage;

			// I happend to know that the TC JSON has only 3 entries.
			for (int i = 0; i < 3; i++)
			{
				string json2POST = NaturalDNA_TestUtilities.LoadTestData_SingleEntity(i);
				StringContent jsonContent = new(json2POST, Encoding.UTF8, "application/json");
				requestMessage = new(HttpMethod.Post, "/api/naturalDNA");
				requestMessage.Content = jsonContent;
				requestMessage.Headers.Add("Authorization", this._accessToken);
				await client.SendAsync(requestMessage);
			}

			// Act
			requestMessage = new(HttpMethod.Get, "/api/naturalDNA");
			requestMessage.Headers.Add("Authorization", this._accessToken);
			HttpResponseMessage response = await client.SendAsync(requestMessage);
			string responseString = response.Content.ReadAsStringAsync().Result;
			JArray jsonArray = JArray.Parse(responseString);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string errorMessage = "There are a different number of entries in the DB than 3!";
			AssertX.Equal(3, jsonArray.Count, errorMessage);

			ITestSuite.MongoDBCollectionCleanup<NaturalDNASequenceEntity>(_mongoClient, _naturalDNACollectionName);
		}
	}
}
