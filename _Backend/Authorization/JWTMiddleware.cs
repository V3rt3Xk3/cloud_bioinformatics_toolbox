using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
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

		public JWTMiddleware(RequestDelegate next)
		{
			this._next = next;
		}

		public async Task Invoke(HttpContext context, IUserService userService, IJWTUtils jwtUtils)
		{
			string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
			string userId = jwtUtils.ValidateAccessToken(token);
			if (userId != null)
			{
				context.Items["User"] = await userService.GetUserByIdAsync(userId);
			}
			await this._next(context);
			return;
		}
	}
}