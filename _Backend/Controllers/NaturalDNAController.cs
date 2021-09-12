using Backend.Models;
using Backend.Services;
using Backend.Authorization;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MongoDB.Bson;

// Asyncs
using System.Threading.Tasks;

namespace Backend.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class NaturalDNAController : ControllerBase
	{
		private readonly INaturalDNAService _NatualDNAService;
		public NaturalDNAController(INaturalDNAService naturalDNAService)
		{
			_NatualDNAService = naturalDNAService;
		}

		[HttpGet(Name = "GetAll")]
		public async Task<ActionResult<List<NaturalDNASequenceEntity>>> Get()
		{
			List<NaturalDNASequenceEntity> response = await _NatualDNAService.GetAsync();
			return response;
		}
		[HttpGet("{id}", Name = "Natural DNA sequence by ID")]
		public async Task<ActionResult<NaturalDNASequenceEntity>> Get(string id)
		{
			bool testParse = ObjectId.TryParse(id, out ObjectId _);


			if (testParse != true)
			{
				return NotFound();
			}
			else
			{
				NaturalDNASequenceEntity sequence = await _NatualDNAService.GetAsyncById(id);
				if (sequence == null) return NotFound();

				return sequence;
			}
		}

		[HttpPost]
		public ActionResult InsertOne([FromBody] NaturalDNASequenceEntity sequence)
		{
			_NatualDNAService.InsertOne(sequence);

			return CreatedAtRoute("Natural DNA sequence by ID", new { id = sequence.Id.ToString() }, sequence);
		}

	}
}