using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

// Application specific implementations
using Backend.Models;
using Backend.Models.Authentication;
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
			(string userId, string tokenId) = jwtUtils.ValidateAccessToken(token);
			if (userId != null)
			{
				UserEntity user = await userService.GetUserByIdAsync(userId);
				if (user.BlackListedJWTs == null) context.Items["User"] = user;
				else if (user.BlackListedJWTs != null && !user.BlackListedJWTs.Where((_token) => _token.TokenID == tokenId).Any())
				{
					context.Items["User"] = user;
				}
			}
			await this._next(context);
			return;
		}
	}
}