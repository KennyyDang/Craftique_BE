using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShipperController : ControllerBase
	{
		private readonly IUserServices _userServices;
		private readonly IOrderServices _orderServices;

		public ShipperController(IUserServices userServices, IOrderServices orderServices)
		{
			_userServices = userServices;
			_orderServices = orderServices;
		}

		[HttpGet("orders")]
		[Authorize(Roles = RolesHelper.Shipper)]
		public async Task<IActionResult> GetShipperOrders([FromQuery] string? status, [FromQuery] TimeModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var currentUserId = Guid.Parse(
				User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
				throw new UnauthorizedAccessException("UserId is missing in the token.")
			);

			var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ??
						   throw new UnauthorizedAccessException("User role is missing in the token.");

			if (userRole != "Shipper")
				return Forbid("Only Shippers can access this endpoint.");

			try
			{
				var result = await _orderServices.GetOrdersAsync(null, status, model, userRole, currentUserId);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while processing your request.");
			}
		}

		[HttpPut("{orderId}/status")]
		[Authorize(Roles = RolesHelper.Shipper)]
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
			if (role != null && role != RolesHelper.Shipper)
			{
				return Unauthorized(new { message = "User not shipper" });
			}

			var validShipperStatuses = new List<string>
					{
						OrderStatusHelper.Delivered
					};

			if (!validShipperStatuses.Contains(model.NewStatus))
			{
				return BadRequest(new { message = $"Invalid status for shipper. Allowed values: {string.Join(", ", validShipperStatuses)}" });
			}

			bool result = await _orderServices.UpdateOrderStatusAsync(orderId, model.NewStatus, user);
			return result
				? Ok(new { message = "Order status updated successfully." })
				: BadRequest(new { message = "Failed to update order status." });
		}

		[HttpGet("all")]
		[Authorize(Roles = RolesHelper.Admin + "," + RolesHelper.Staff)]
		public async Task<IActionResult> GetAllShippersWithPendingOrders()
		{
			try
			{
				var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
				if (userRole != RolesHelper.Admin && userRole != RolesHelper.Staff)
				{
					return Forbid("Only Admin or Staff can access this endpoint.");
				}

				var shippers = await _userServices.GetAllShippersWithPendingOrdersAsync();
				var sortedShippers = shippers.OrderBy(s => s.PendingOrdersCount);
				return Ok(sortedShippers);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
