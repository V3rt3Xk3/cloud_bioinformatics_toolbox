using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

using Xunit;
using Xunit.Extensions.Ordering;
using Newtonsoft.Json;
using MongoDB.Driver;

using Backend.Models;

using BackendTests.Utilities;

namespace BackendTests
{
	public class NaturalDNAControllerTests : IClassFixture<CustomWebApplicationFactory<Backend.Startup>>, IDisposable
	{
		private readonly CustomWebApplicationFactory<Backend.Startup> _factory;
		public NaturalDNAControllerTests(CustomWebApplicationFactory<Backend.Startup> factory)
		{
			_factory = factory;
		}

		// NOTE: This code runs after each test.
		public void Dispose()
		{
			string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";
			MongoClient client = new MongoClient(connectionString);

			string DatabaseNamespace = "cloud_bioinformatics_test";
			client.DropDatabase(DatabaseNamespace);
		}

		[Theory, Order(1)]
		[InlineData("/api/naturalDNA")]
		public async Task CheckingForEndpoint_StatusCodes_ContentType(string url)
		{
			// Arrange
			System.Net.Http.HttpClient client = _factory.CreateClient();

			// Act
			System.Net.Http.HttpResponseMessage response = await client.GetAsync(url);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string errorMessage = "The repsonse content is not: 'application/json; charset=utf-8'";
			AssertX.Equal("application/json; charset=utf-8",
							response.Content.Headers.ContentType.ToString(),
							errorMessage);
		}

		[Fact, Order(2)]
		public async Task CheckingFor_InsertingOne_ReceivingTheInput()
		{
			// Arrange
			HttpClient client = _factory.CreateClient();

			string json2POST = NaturalDNA_TestUtilities.LoadTestData_SingleEntity();
			StringContent jsonContent = new StringContent(json2POST, Encoding.UTF8, "application/json");

			// Act
			HttpResponseMessage response = await client.PostAsync("/api/naturalDNA", jsonContent);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);

			string errorMessage = "The inserted Sequence document had a different \"sequenceName\" than \"Test 1\"";
			AssertX.Equal("Sus scrofa breed Landrace oxytocin gene", jsonResponse["sequenceName"], errorMessage);
		}
		[Fact, Order(3)]
		public async Task CheckingFor_MultipleInsertingOne()
		{
			// Arrange
			System.Net.Http.HttpClient client = _factory.CreateClient();

			// I happend to know that the TC JSON has only 3 entries.
			for (int i = 0; i < 3; i++)
			{
				string json2POST = NaturalDNA_TestUtilities.LoadTestData_SingleEntity(i);
				StringContent jsonContent = new StringContent(json2POST, Encoding.UTF8, "application/json");
				await client.PostAsync("/api/naturalDNA", jsonContent);
			}


			// Act
			HttpResponseMessage response = await client.GetAsync("/api/naturalDNA");
			string responseString = response.Content.ReadAsStringAsync().Result;
			JArray jsonArray = JArray.Parse(responseString);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string errorMessage = "There are a different number of entries in the DB than 3!";
			AssertX.Equal(3, jsonArray.Count, errorMessage);
		}
	}
}
