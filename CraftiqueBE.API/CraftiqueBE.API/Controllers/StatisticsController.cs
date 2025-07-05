using CraftiqueBE.Data.Helper;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
	public class StatisticsController : ControllerBase
	{
		private readonly IOrderServices _orderServices;

		public StatisticsController(IOrderServices orderServices)
		{
			_orderServices = orderServices;
		}

		[HttpGet("orders")]
		public async Task<IActionResult> GetOrderStatistics()
		{
			var result = await _orderServices.GetOrderStatisticsAsync();
			return Ok(result);
		}
	}
}
