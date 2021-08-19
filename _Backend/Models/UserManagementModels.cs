using System.ComponentModel.DataAnnotations;

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
		public int Id { get; set; }
		public string Username { get; set; }
		public string JWTToken { get; set; }
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