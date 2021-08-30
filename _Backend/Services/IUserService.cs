using System.Collections.Generic;
using System.Threading.Tasks;

using Backend.Models.UserManagement;
using Backend.Models;


namespace Backend.Services
{
	public interface IUserService
	{
		// Authentication
		Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
		Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
		Task RevokeToken(string token, string ipAddress);

		// User methods
		Task<List<UserEntity>> GetAllUsersAsync();
		Task<UserEntity> GetUserByIdAsync(string id);
		Task Register(RegisterRequest model);
	}
}