using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

// Application specific implementations
using Backend.Helpers;
using Backend.Services;

namespace Backend.Authorization
{
	public class JWTMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly AppSettings _appSettings;

		public JWTMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
		{
			this._next = next;
			this._appSettings = appSettings.Value;
		}

		public async Task Invoke(HttpContext context, IUserService userService, IJWTUtils jWTUtils)
		{

		}
	}
}