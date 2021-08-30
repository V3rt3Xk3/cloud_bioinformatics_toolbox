using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Backend.Models;
using Backend.Services;
using Backend.Helpers;
using Backend.Authorization;

namespace Backend
{
	public class Startup
	{
		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors((_options) =>
			{
				_options.AddPolicy(name: MyAllowSpecificOrigins,
									(_builder) =>
									{
										_builder
											.WithOrigins("http://localhost:3000")
											.AllowAnyHeader()
											.AllowAnyMethod()
											.AllowCredentials();
									});
			});

			// Requires using Microsoft.Extensions.Options
			services.Configure<CloudBioinformaticsDatabaseSettings>(
				Configuration.GetSection(nameof(CloudBioinformaticsDatabaseSettings))
			);
			services.AddSingleton<ICloudBioinformaticsDatabaseSettings>(sp =>
				sp.GetRequiredService<IOptions<CloudBioinformaticsDatabaseSettings>>().Value
			);
			// NOTE: Configuring the AppSettings
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

			// NOTE: Mappers
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


			// NOTE: Controllers
			services.AddControllers();

			// NOTE: These will be the controller services
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<INaturalDNAService, NaturalDNAService>();
			// NOTE: JWT Utils
			services.AddScoped<IJWTUtils, JWTUtils>();


			services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(MyAllowSpecificOrigins);

			// HACK: Error handling middleware
			app.UseMiddleware<ErrorHandlerMiddleware>();

			// // WOW: Turning off the Microsoft Auth middleware -> To add the one we implemented
			// app.UseAuthorization();
			app.UseMiddleware<JWTMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
