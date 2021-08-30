using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Backend.Models.UserManagement;


namespace Backend.Controllers
{
	public interface IUserController
	{
		// Authentication methods
		Task<IActionResult> Register([FromBody] RegisterRequest model);
		Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model);

		// Refresh token methods
		Task<IActionResult> RefreshToken();
		Task<IActionResult> RevokeToken(RevokeTokenRequest model);

		// User management methods
		Task<IActionResult> GetAllUsersAsync();
		Task<IActionResult> GetUserByIdAsync();
	}
}