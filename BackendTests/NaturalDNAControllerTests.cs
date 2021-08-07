using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;

using Xunit;

using Backend;
using Backend.Models;
using Backend.Services;
using Backend.Controllers;

namespace BackendTests
{
	public class NaturalDNAControllerTests : IClassFixture<WebApplicationFactory<Backend.Startup>>
	{
		private readonly WebApplicationFactory<Backend.Startup> _factory;
		public NaturalDNAControllerTests(WebApplicationFactory<Backend.Startup> factory)
		{
			_factory = factory;
		}

		[Theory]
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
		[Fact]
		public async Task NaturalDNAGet_ShouldReturn_0_Length()
		{
			// Arrange
			System.Net.Http.HttpClient client = _factory.CreateClient();

			// Act
			System.Net.Http.HttpResponseMessage response = await client.GetAsync("/api/naturalDNA");
			var responseString = response.Content.ReadAsStringAsync().Result;
			JArray jsonArray = JArray.Parse(responseString);

			// Assert
			response.EnsureSuccessStatusCode(); // Status code 200-299
			Assert.Equal(5, jsonArray.Count);
		}
	}
}
