using System.Linq;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Backend.Models;
using Backend.Services;
using Backend.Controllers;

namespace BackendTests
{
	public class CustomWebApplicationFactory<TStartup>
	: WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			// Before TStartup ConfigureServices
			builder.ConfigureServices(services =>
			{
			});

			// After TStartup ConfigureServices
			builder.ConfigureTestServices(services =>
			{
				services.Configure<CloudBioinformaticsDatabaseSettings>(settings =>
				{
					string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";
					settings.ConnectionString = connectionString;
					settings.DatabaseName = "cloud_bioinformatics_test";
				});

				ServiceDescriptor serviceDescriptor = services.SingleOrDefault(
				descriptor => descriptor.ServiceType == typeof(NaturalDNAController));

				services.Remove(serviceDescriptor);
				services.AddSingleton<NaturalDNAService>();
			});
		}
	}
}
