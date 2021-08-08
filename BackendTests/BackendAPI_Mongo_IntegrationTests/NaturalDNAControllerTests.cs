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
		public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
		{
			// Arrange
			System.Net.Http.HttpClient client = _factory.CreateClient();

			// Act
			System.Net.Http.HttpResponseMessage response = await client.GetAsync(url);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			Assert.Equal("application/json; charset=utf-8",
							response.Content.Headers.ContentType.ToString());
		}

		[Fact, Order(2)]
		public async Task NaturalDNA_InsertOne()
		{
			// Arrange
			HttpClient client = _factory.CreateClient();

			NaturalDNASequence sequence2POST = new NaturalDNASequence();
			sequence2POST.sequenceName = "NaturalDNA POST_01";
			sequence2POST.sequence = "AGATCGATCGGCGAGCTA";
			string json2POST = JsonConvert.SerializeObject(sequence2POST);
			StringContent jsonContent = new StringContent(json2POST, Encoding.UTF8, "application/json");

			// Act
			HttpResponseMessage response = await client.PostAsync("/api/naturalDNA", jsonContent);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			string responseString = response.Content.ReadAsStringAsync().Result;
			JObject jsonResponse = JObject.Parse(responseString);
			Assert.Equal(jsonResponse["sequenceName"], "NaturalDNA POST_01");
		}
		[Fact, Order(3)]
		public async Task NaturalDNAGet_ShouldReturn_1_Length()
		{
			// Arrange
			System.Net.Http.HttpClient client = _factory.CreateClient();

			NaturalDNASequence sequence2POST = new NaturalDNASequence();
			sequence2POST.sequenceName = "NaturalDNA POST_01";
			sequence2POST.sequence = "AGATCGATCGGCGAGCTA";
			string json2POST = JsonConvert.SerializeObject(sequence2POST);
			StringContent jsonContent = new StringContent(json2POST, Encoding.UTF8, "application/json");
			await client.PostAsync("/api/naturalDNA", jsonContent);

			// Act
			HttpResponseMessage response = await client.GetAsync("/api/naturalDNA");
			string responseString = response.Content.ReadAsStringAsync().Result;
			JArray jsonArray = JArray.Parse(responseString);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			Console.WriteLine(responseString);
			Assert.Equal(1, jsonArray.Count);
		}
	}
}
