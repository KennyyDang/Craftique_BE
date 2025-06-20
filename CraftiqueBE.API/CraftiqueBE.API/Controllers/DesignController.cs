using CraftiqueBE.Data.Models.DesignUploadModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CraftiqueBE.Data.Helper;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DesignController : ControllerBase
	{
		private readonly IDesignService _designService;

		public DesignController(IDesignService designService)
		{
			_designService = designService;
		}

		// POST: api/Design/upload
		[HttpPost("upload")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> UploadDesign([FromForm] DesignUploadRequestModel request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated");

			var result = await _designService.UploadDesignAsync(userId, request);
			return Ok(result);
		}

		// POST: api/Design/customize
		[HttpPost("customize")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> CreateCustomization([FromBody] CreateCustomizationModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _designService.CreateCustomizationAsync(model);
			return Ok(result);
		}
	}
}
