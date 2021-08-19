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
		private readonly IMongoCollection<NaturalDNASequenceEntity> _naturalDNASequences;
		private readonly IMongoDatabase database;
		public string CollectionName { get; }

		public NaturalDNAService(ICloudBioinformaticsDatabaseSettings settings)
		{
			MongoClient client = new MongoClient(settings.ConnectionString);
			this.database = client.GetDatabase(settings.DatabaseName);
			this.CollectionName = settings.NaturalDNASequences_CollectionName;

			_naturalDNASequences = database.GetCollection<NaturalDNASequenceEntity>(this.CollectionName);
		}
		public async Task<List<NaturalDNASequenceEntity>> GetAsync()
		{
			IAsyncCursor<NaturalDNASequenceEntity> requestResults = await _naturalDNASequences.FindAsync(NaturalDNASequence => true);
			return await requestResults.ToListAsync();
		}
		public async Task<NaturalDNASequenceEntity> GetAsyncById(string id)
		{
			IAsyncCursor<NaturalDNASequenceEntity> requestResults = await _naturalDNASequences.FindAsync<NaturalDNASequenceEntity>(
																						sequence => sequence.Id == id);
			return await requestResults.FirstOrDefaultAsync<NaturalDNASequenceEntity>();
		}

		public void InsertOne(NaturalDNASequenceEntity sequence)
		{
			if (sequence == null)
			{
				throw new ArgumentNullException(nameof(sequence));
			}

			_naturalDNASequences.InsertOne(sequence);
		}

	}
}