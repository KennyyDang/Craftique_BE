using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminServices _adminServices;
		private readonly IUserServices _userServices;
		private readonly IOrderServices _orderServices;

		public AdminController(IAdminServices adminServices, IUserServices userServices, IOrderServices orderServices)
		{
			_adminServices = adminServices;
			_userServices = userServices;
			_orderServices = orderServices;
		}

		[HttpGet("get-total-user")]
		public async Task<int> GetTotalUserAsync()
		{
			var result = await _adminServices.GetTotalUserAsync();

			return result;
		}



		[HttpGet("get-total-revenue")]
		public async Task<IActionResult> GetTotalRevenueAsync([FromQuery] TimeModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _adminServices.GetTotalRevenueAsync(model);
			return Ok(result);
		}

		[HttpGet("total-products")]
		public async Task<IActionResult> GetTotalProductItems([FromQuery] CategoryProductFilterModel filter)
		{
			var result = await _adminServices.GetTotalProductItemsAsync(filter);
			return Ok(result);
		}



		[HttpGet("get-top-selling-product-items")]
		public async Task<IActionResult> GetTopSellingProductItemsAsync([FromQuery] TimeModel model, int? topN)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _adminServices.GetTopSellingProductItemsAsync(model, topN);
			return Ok(result);
		}

		[HttpGet("get-top-costumers")]
		public async Task<IActionResult> GetTopCustomersAsync([FromQuery] TimeModel model, int? topN)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _adminServices.GetTopCustomersAsync(model, topN);
			return Ok(result);
		}

		[HttpPut("{orderId}/status")]
		public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] UpdateOrderStatusModel model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User not authenticated" });
			}

			var (user, role) = await _userServices.GetUserWithRoleAsync(userId);
			if (user == null)
			{
				return Unauthorized(new { message = "User not found" });
			}
			if (role != null && !(role == RolesHelper.Admin || role == RolesHelper.Staff))
			{
				return Unauthorized(new { message = "User not admin or staff" });
			}

			// Validate status for staff operations
			var validStaffStatuses = new List<string>
				{
					OrderStatusHelper.Processing,
					OrderStatusHelper.Cancelled,
					OrderStatusHelper.Shipped,
					OrderStatusHelper.Completed,
					OrderStatusHelper.Refunded
				};

			if (!validStaffStatuses.Contains(model.NewStatus))
			{
				return BadRequest(new { message = $"Invalid status for staff. Allowed values: {string.Join(", ", validStaffStatuses)}" });
			}

			// When changing to Shipped status, shipper must be assigned
			if (model.NewStatus == OrderStatusHelper.Shipped && string.IsNullOrEmpty(model.ShipperId))
			{
				return BadRequest(new { message = "A shipper must be assigned when changing status to Shipped." });
			}

			bool result = await _orderServices.UpdateOrderStatusAsync(orderId, model.NewStatus, user, model.ShipperId);
			return result
				? Ok(new { message = "Order status updated successfully." })
				: BadRequest(new { message = "Failed to update order status." });
		}

	}
}
