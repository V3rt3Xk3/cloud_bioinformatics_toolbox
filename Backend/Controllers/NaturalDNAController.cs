using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
		public ActionResult<List<NaturalDNASequence>> Get() =>
			_NatualDNAService.Get();
	}
}