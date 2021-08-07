using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// Asyncs
using System.Threading.Tasks;

namespace Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NaturalDNAController : ControllerBase
	{
		private readonly NaturalDNAService _NatualDNAService;
		public NaturalDNAController(NaturalDNAService naturalDNAService)
		{
			_NatualDNAService = naturalDNAService;
		}

		[HttpGet]
		public async Task<ActionResult<List<NaturalDNASequence>>> Get()
		{
			List<NaturalDNASequence> response = await _NatualDNAService.GetAsync();
			return response;
		}

	}
}