

namespace Backend.Models
{
	public class CloudBioinformaticsDatabaseSettings : ICloudBioinformaticsDatabaseSettings
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }
		public string Users_CollectionName { get; set; }
		public string NaturalDNASequences_CollectionName { get; set; }
	}
	public interface ICloudBioinformaticsDatabaseSettings
	{
		string ConnectionString { get; set; }
		string DatabaseName { get; set; }
		string Users_CollectionName { get; set; }
		string NaturalDNASequences_CollectionName { get; set; }
	}
}