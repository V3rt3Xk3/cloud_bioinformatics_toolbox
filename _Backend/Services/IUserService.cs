using System.Collections.Generic;
using System.Threading.Tasks;

using Backend.Models.UserManagement;
using Backend.Models;


namespace Backend.Services
{
	public interface IUserService
	{
		Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
		// Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
		Task<List<UserEntity>> GetAsync();
		Task<UserEntity> GetAsyncById(string id);
		Task Register(RegisterRequest model);
	}
}