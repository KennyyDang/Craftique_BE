using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomProductFileController : ControllerBase
	{
		private readonly ICustomProductFileService _customProductFileService;

		public CustomProductFileController(ICustomProductFileService customProductFileService)
		{
			_customProductFileService = customProductFileService;
		}

		// ➤ Customer upload file / text
		[Authorize(Roles = RolesHelper.Customer)]
		[HttpPost("upload")]
		public async Task<IActionResult> UploadFile([FromForm] CustomProductFileUploadModel model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated");

			var result = await _customProductFileService.UploadFileAsync(model, userId);
			return Ok(result);
		}

		// ➤ Admin, Staff xem file của 1 custom product
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		[HttpGet("{customProductId}")]
		public async Task<IActionResult> GetFiles(int customProductId)
		{
			var result = await _customProductFileService.GetFilesByCustomProductAsync(customProductId);
			return Ok(result);
		}

		// ➤ Customer xem tất cả file của mình
		[Authorize(Roles = RolesHelper.Customer)]
		[HttpGet("mine")]
		public async Task<IActionResult> GetMyFiles()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated");

			var result = await _customProductFileService.GetCustomProductFilesByUserAsync(userId);
			return Ok(result);
		}

		// ➤ Admin hoặc user xoá file
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}, {RolesHelper.Customer}")]
		[HttpDelete("{customProductFileId}")]
		public async Task<IActionResult> DeleteFile(int customProductFileId)
		{
			var success = await _customProductFileService.DeleteFileAsync(customProductFileId);
			if (!success) return BadRequest("Xoá thất bại.");
			return Ok(new { message = "Đã xoá thành công." });
		}

		// ➤ Admin & Staff: lấy tất cả custom product files
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		[HttpGet("all")]
		public async Task<IActionResult> GetAllFiles()
		{
			var result = await _customProductFileService.GetAllFilesAsync();
			return Ok(result);
		}

		// ➤ Admin & Staff: lấy 1 custom product file theo ID
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}, {RolesHelper.Customer}")]
		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetFileById(int id)
		{
			var result = await _customProductFileService.GetFileByIdAsync(id);
			if (result == null) return NotFound("Không tìm thấy file.");
			return Ok(result);
		}
	}
}
