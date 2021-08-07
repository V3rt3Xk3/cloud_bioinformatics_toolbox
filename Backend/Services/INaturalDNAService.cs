using System.Collections.Generic;
using Backend.Models;

// Asyncs
using System.Threading.Tasks;



namespace Backend.Services
{
	public interface INaturalDNAService
	{
		#region GetAsync
		Task<List<NaturalDNASequence>> GetAsync();
		Task<NaturalDNASequence> GetAsync(string id);
		#endregion

		#region InsertOneAsync
		// NOTE: The reasoning behind using a Sync not Async is that it is not IO bound but computation bound code. We need the Database to respond before letting the user move forward.
		void InsertOne(NaturalDNASequence sequence);
		#endregion
	}
}