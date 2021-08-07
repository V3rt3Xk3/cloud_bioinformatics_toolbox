using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

// Asyncs
using System.Threading.Tasks;

namespace Backend.Services
{
	public class NaturalDNAService : INaturalDNAService
	{
		private readonly IMongoCollection<NaturalDNASequence> _naturalDNASequences;

		public NaturalDNAService(ICloudBioinformaticsDatabaseSettings settings)
		{
			MongoClient client = new MongoClient(settings.ConnectionString);
			IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

			_naturalDNASequences = database.GetCollection<NaturalDNASequence>(settings.NaturalDNASequences_CollectionName);
		}
		public async Task<List<NaturalDNASequence>> GetAsync()
		{
			IAsyncCursor<NaturalDNASequence> requestResults = await _naturalDNASequences.FindAsync(NaturalDNASequence => true);
			return await requestResults.ToListAsync();
		}

	}
}