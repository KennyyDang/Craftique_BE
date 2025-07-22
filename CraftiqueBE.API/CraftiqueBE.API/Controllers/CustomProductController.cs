using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomProductController : ControllerBase
	{
		private readonly ICustomProductService _customProductService;

		public CustomProductController(ICustomProductService customProductService)
		{
			_customProductService = customProductService;
		}

		// ➤ Admin thêm custom product
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		[HttpPost("admin")]
		public async Task<IActionResult> AddCustomProduct([FromBody] CustomProductUploadModel model)
		{
			var result = await _customProductService.AddCustomProductWithImageAsync(model);
			return Ok(result);
		}

		// ➤ Admin, Staff xem tất cả custom products
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}, {RolesHelper.Customer}")]
		[HttpGet]
		public async Task<IActionResult> GetAllCustomProducts()
		{
			var result = await _customProductService.GetAllCustomProductsAsync();
			return Ok(result);
		}

		// ➤ Admin xoá custom product
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCustomProduct(int id)
		{
			var success = await _customProductService.DeleteCustomProductAsync(id);
			if (!success) return BadRequest("Xoá thất bại.");
			return Ok(new { message = "Đã xoá thành công." });
		}

		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}, {RolesHelper.Customer}")]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCustomProductById(int id)
		{
			var products = await _customProductService.GetAllCustomProductsAsync();
			var product = products.FirstOrDefault(p => p.CustomProductID == id);
			if (product == null)
				return NotFound();

			return Ok(product);
		}

	}
}
