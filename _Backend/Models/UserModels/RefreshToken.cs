using System;

namespace Backend.Models.Authentication
{
	public class RefreshToken
	{
		public string Token { get; set; }
		public string IssuedJWTTokenId { get; set; }
		public DateTime Expires { get; set; }
		public DateTime Created { get; set; }
		public string CreatedByIp { get; set; }
		public DateTime? Revoked { get; set; }
		public string RevokedByIp { get; set; }
		public string ReplacedByToken { get; set; }
		public string ReasonsRevoked { get; set; }
		// public bool IsExpired => (DateTime.UtcNow >= Expires);
		// public bool IsRevoked => Revoked != null;
		// public bool IsActive => (!IsRevoked && !IsExpired);
	}
}