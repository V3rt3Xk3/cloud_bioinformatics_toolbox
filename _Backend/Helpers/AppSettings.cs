namespace Backend.Helpers
{
	public class AppSettings
	{
		public string Secret { get; set; }
		public int RefreshTokenTTL { get; set; }
		public int RefreshTokenExpiresDuration { get; set; }
	}
}