using System;

namespace Backend.Models.Authentication
{
	public class BlackListedJWT
	{
		public string TokenID { get; set; }
		public string FirstAncestor { get; set; }
		public int AttemptsToReuse { get; set; }
		public DateTime IssueDateTime { get; set; }
		public string BlackListedByIp { get; set; }
		public string CorrespondingRefreshToken { get; set; }

		public BlackListedJWT() {}

		public BlackListedJWT(	string tokenID, 
								string firstAncestor, 
								DateTime issueDateTime, 
								string ipAddresss, 
								string correspondingRefreshToken,
								int attemptsToReuse = 0)
		{
			this.TokenID = tokenID;
			this.FirstAncestor = firstAncestor;
			this.AttemptsToReuse = attemptsToReuse;
			this.IssueDateTime = issueDateTime;
			this.BlackListedByIp = ipAddresss;
			this.CorrespondingRefreshToken = correspondingRefreshToken;
		}
	}
}