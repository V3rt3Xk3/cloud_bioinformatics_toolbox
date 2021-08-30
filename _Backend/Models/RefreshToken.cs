using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

using System;

namespace Backend.Models
{
	public class RefreshToken
	{
		public string Token { get; set; }
		public DateTime Expires { get; set; }
		public DateTime Created { get; set; }
		public string CreatedByIp { get; set; }
		public DateTime? Revoked { get; set; }
		public string RevokedByIp { get; set; }
		public string ReplacedByToken { get; set; }
		public string ReasonsRevoked { get; set; }
		public bool IsExpired
		{
			get { return (DateTime.UtcNow >= Expires); }
		}
		public bool IsRevoked
		{
			get { return (Revoked != null); }
		}
		public bool IsActive
		{
			get { return (!IsRevoked && !IsExpired); }
		}
	}
}