using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services
{
	public class NaturalDNAService
	{
		private readonly IMongoCollection<NaturalDNASequence> _naturalDNASequences;

		public NaturalDNAService(ICloudBioinformaticsDatabaseSettings settings)
		{
			MongoClient client = new MongoClient(settings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

			_naturalDNASequences = database.GetCollection<NaturalDNASequence>(settings.NaturalDNASequences_CollectionName);
		}
		public List<NaturalDNASequence> Get() =>
			_naturalDNASequences.Find(NaturalDNASequence => true).ToList();
	}
}