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

		[HttpGet(Name = "GetAll")]
		public async Task<ActionResult<List<NaturalDNASequence>>> Get()
		{
			List<NaturalDNASequence> response = await _NatualDNAService.GetAsync();
			return response;
		}
		[HttpGet("{id}", Name = "Natural DNA sequence by ID")]
		public async Task<ActionResult<NaturalDNASequence>> Get(string id)
		{
			NaturalDNASequence sequence = await _NatualDNAService.GetAsync(id);

			if (sequence == null)
			{
				return NotFound();
			}
			return sequence;
		}

		[HttpPost]
		public ActionResult InsertOne([FromBody] NaturalDNASequence sequence)
		{
			_NatualDNAService.InsertOne(sequence);

			return CreatedAtRoute("Natural DNA sequence by ID", new { id = sequence.Id.ToString() }, sequence);
		}

	}
}