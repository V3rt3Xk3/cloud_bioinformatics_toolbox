using Backend.Models;
using MongoDB.Driver;

using System;
using System.Collections.Generic;

// Asyncs
using System.Threading.Tasks;

namespace Backend.Services
{
	public class NaturalDNAService : INaturalDNAService
	{
		private readonly IMongoCollection<NaturalDNASequence> _naturalDNASequences;
		private readonly IMongoDatabase database;
		public string CollectionName { get; }

		public NaturalDNAService(ICloudBioinformaticsDatabaseSettings settings)
		{
			MongoClient client = new MongoClient(settings.ConnectionString);
			this.database = client.GetDatabase(settings.DatabaseName);
			this.CollectionName = settings.NaturalDNASequences_CollectionName;

			_naturalDNASequences = database.GetCollection<NaturalDNASequence>(this.CollectionName);
		}
		public async Task<List<NaturalDNASequence>> GetAsync()
		{
			IAsyncCursor<NaturalDNASequence> requestResults = await _naturalDNASequences.FindAsync(NaturalDNASequence => true);
			return await requestResults.ToListAsync();
		}
		public async Task<NaturalDNASequence> GetAsync(string id)
		{
			IAsyncCursor<NaturalDNASequence> requestResults = await _naturalDNASequences.FindAsync<NaturalDNASequence>(
																						sequence => sequence.Id == id);
			return await requestResults.FirstAsync<NaturalDNASequence>();
		}

		public void InsertOne(NaturalDNASequence sequence)
		{
			if (sequence == null)
			{
				throw new ArgumentNullException(nameof(sequence));
			}

			_naturalDNASequences.InsertOne(sequence);
		}

	}
}