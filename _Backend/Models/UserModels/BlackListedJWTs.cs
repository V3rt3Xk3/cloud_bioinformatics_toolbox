using System;

namespace Backend.Models.Authentication
{
	public class BlackListedJWT
	{
		public string TokenID { get; set; }
		public int AttemptsToReuse { get; set; }
		public DateTime IssueDateTime { get; set; }
		public string BlackListedByIp { get; set; }
		public string CorrespondingRefreshToken { get; set; }
	}
}