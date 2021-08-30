using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Backend.Models.UserManagement
{
	public class AuthenticateRequest
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}

	public class AuthenticateResponse
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string JWTToken { get; set; }
		[JsonIgnore]
		public string RefreshToken { get; set; }

		public AuthenticateResponse(UserEntity _user, string JWTToken, string refreshToken)
		{
			this.Id = _user.Id;
			this.Username = _user.Username;
			this.JWTToken = JWTToken;
			this.RefreshToken = refreshToken;
		}
	}

	public class RegisterRequest
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string RePassword { get; set; }
	}

	public class UpdateRequest
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string RePassword { get; set; }
	}
}