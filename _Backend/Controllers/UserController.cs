using Backend.Models;
using Backend.Models.UserManagement;
using Backend.Services;
using Backend.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using MongoDB.Bson;

// Asyncs
using System.Threading.Tasks;

namespace Backend.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		public UsersController(IUserService userService)
		{
			this._userService = userService;
		}

		[AllowAnonymous]
		[HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
		{
			AuthenticateResponse response = await _userService.Authenticate(model, IpAddress());
			SetTokenCookie(response.RefreshToken);
			return Ok(response);
		}

		[AllowAnonymous]
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			string refreshToken = Request.Cookies["refreshToken"];
			AuthenticateResponse response = await _userService.RefreshToken(refreshToken, IpAddress());
			SetTokenCookie(response.RefreshToken);
			return Ok(response);
		}


		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			await _userService.Register(model);
			return Ok(new { message = "Registration successful" });
		}

		// WOW: Helper methods
		// BUG: This method needs to be used, so it can be saved.
		private void SetTokenCookie(string token)
		{
			CookieOptions _cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = System.DateTime.UtcNow.AddDays(7)
			};
			Response.Cookies.Append("refreshToken", token, _cookieOptions);
		}

		private string IpAddress()
		{
			// get source ip address for the current request
			if (Request.Headers.ContainsKey("X-Forwarded-For"))
				return Request.Headers["X-Forwarded-For"];
			else
				return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		}

	}
}