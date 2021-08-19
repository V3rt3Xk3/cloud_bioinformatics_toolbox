using Backend.Models;
using Backend.Models.UserManagement;
using Backend.Services;
using Backend.Authorization;

using Microsoft.AspNetCore.Mvc;
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
			AuthenticateResponse response = await _userService.Authenticate(model);
			return Ok(response);
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			await _userService.Register(model);
			return Ok(new { message = "Registration successful" });
		}
	}
}