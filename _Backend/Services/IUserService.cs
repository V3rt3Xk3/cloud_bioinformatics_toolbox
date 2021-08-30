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
		Task<List<UserEntity>> GetAsync();
		Task<UserEntity> GetAsyncById(string id);
		Task Register(RegisterRequest model);
		private Task<UserEntity> DoesUserExistsByUsername(string username);
		private Task<UserEntity> getUserByRefreshToken(string token);

		// Token methods
		private Task<RefreshToken> RotateRefreshToken(UserEntity user, RefreshToken oldRefreshToken, string ipAddress);
		private Task RemoveOldRefreshTokens(UserEntity user);
		private Task revokeDescendantRefreshTokens(RefreshToken refreshToken, UserEntity user, string ipAddress, string reason);
		private Task RevokeRefreshToken(UserEntity user, RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null);
	}
}