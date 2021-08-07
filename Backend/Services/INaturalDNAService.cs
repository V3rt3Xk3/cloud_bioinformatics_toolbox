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
		#endregion
	}
}