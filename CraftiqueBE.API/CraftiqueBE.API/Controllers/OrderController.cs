using AutoMapper;
using CraftiqueBE.API.Hubs;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using CraftiqueBE.Data.ViewModels.OrderVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class OrderController : ControllerBase
	{
		private readonly IOrderServices _orderServices;
		private readonly IUserServices _userServices;
		private readonly IMapper _mapper;
		private readonly IHubContext<NotificationHub> _notificationHub;
		private readonly INotificationServices _notificationServices;
		private readonly UserManager<User> _userManager;

		public OrderController(
			IOrderServices orderServices,
			IUserServices userServices,
			IMapper mapper,
			IHubContext<NotificationHub> notificationHub,
			INotificationServices notificationServices,
			UserManager<User> userManager)
		{
			_orderServices = orderServices;
			_userServices = userServices;
			_mapper = mapper;
			_notificationHub = notificationHub;
			_notificationServices = notificationServices;
			_userManager = userManager;
		}

		[HttpGet("orders")]
		public async Task<IActionResult> GetOrders([FromQuery] Guid? userId, [FromQuery] string? status, [FromQuery] TimeModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var currentUserId = Guid.Parse(
			User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
			throw new UnauthorizedAccessException("UserId is missing in the token.")
			);
			var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ??
						   throw new UnauthorizedAccessException("User role is missing in the token.");

			var result = await _orderServices.GetOrdersAsync(userId, status, model, userRole, currentUserId);
			return Ok(result);
		}


		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderById(int id)
		{
			var order = await _orderServices.GetByIdAsync(id);
			var orderViewModel = _mapper.Map<OrderViewModel>(order);
			return Ok(orderViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> AddOrder([FromBody] OrderModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var order = await _orderServices.AddAsync(model);
			var orderViewModel = _mapper.Map<OrderViewModel>(order);

			try
			{
				var customerName = "Unknown";

				if (!string.IsNullOrEmpty(model.UserID))
				{
					var user = await _userManager.FindByIdAsync(model.UserID);
					if (user != null)
					{
						customerName = user.UserName ?? user.Email ?? "Customer";
					}
				}

				var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
				var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

				foreach (var user in adminUsers.Union(staffUsers))
				{
					var header = "New Order Placed";
					var content = $"Customer {customerName} has placed a new order (ID: {order.OrderID})";

					var notification = await _notificationServices.CreateNotification(user.Id, header, content);

					await _notificationHub.Clients.Group($"User_{user.Id}").SendAsync(
						"ReceiveNotification",
						notification
					);

					var unreadCount = await _notificationServices.GetUnreadCount(user.Id);
					await _notificationHub.Clients.Group($"User_{user.Id}").SendAsync(
						"UpdateUnreadCount",
						unreadCount
					);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending order notification: {ex.Message}");
			}

			return CreatedAtAction(nameof(GetOrderById), new { id = orderViewModel.OrderID }, orderViewModel);
		}

		[HttpPut("{orderId}/status")]
		[Authorize]
		public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] UpdateOrderStatusModel model, [FromQuery] bool isCancelledByCustomer = false)
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

			var validStatuses = new List<string>
				{
					OrderStatusHelper.Pending,
					OrderStatusHelper.Paid,
					OrderStatusHelper.Processing,
					OrderStatusHelper.Shipped,
					OrderStatusHelper.Delivered,
					OrderStatusHelper.Completed,
					OrderStatusHelper.Cancelled,
					OrderStatusHelper.RefundRequested,
					OrderStatusHelper.Refunded
				};

			if (!validStatuses.Contains(model.NewStatus.Trim()))
			{
				return BadRequest(new { message = $"Invalid status '{model.NewStatus}'. Allowed values: {string.Join(", ", validStatuses)}" });
			}

			bool result = await _orderServices.UpdateOrderStatusAsync(orderId, model.NewStatus.Trim(), user, model.ShipperId);

			if (result)
			{
				var responseDict = new Dictionary<string, object>
				{
					{ "message", "Order status updated successfully." },
					{ "userId", string.Empty }
				};

				try
				{
					var order = await _orderServices.GetByIdAsync(orderId);
					var customerUser = await _userManager.FindByIdAsync(order.UserID);
					var customerName = customerUser?.UserName ?? customerUser?.Email ?? "Unknown Customer";

					responseDict["userId"] = order.UserID;

					if (model.NewStatus == OrderStatusHelper.Cancelled)
					{
						var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
						var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

						foreach (var adminStaffUser in adminUsers.Union(staffUsers))
						{
							var header = "Order Cancelled";
							var content = $"Customer {customerName} has cancelled order (ID: {orderId})";

							var notification = await _notificationServices.CreateNotification(adminStaffUser.Id, header, content);

							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(adminStaffUser.Id);
							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}

						if (customerUser != null)
						{
							if (!isCancelledByCustomer)
							{
								var header = "Order Cancelled";
								var content = $"Dear customer, your Order (ID: {orderId}) has been cancelled by our moderators.";

								var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
							else
							{
								var header = "Order Cancelled";
								var content = $"Your Order (ID: {orderId}) has been cancelled successfully.";

								var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
						}
					}
					else if (model.NewStatus == OrderStatusHelper.Processing)
					{
						if (customerUser != null)
						{
							var header = "Order Confirmed";
							var content = $"Dear customer, your Order (ID: {orderId}) has been confirmed. A shipper will be assigned soon to deliver to you.";

							var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}
					}
					else if (model.NewStatus == OrderStatusHelper.Shipped)
					{
						var shipperUser = await _userManager.FindByIdAsync(model.ShipperId);
						var shipperName = shipperUser?.UserName ?? "Unknown";
						var shipperPhone = shipperUser?.PhoneNumber ?? "N/A";

						responseDict["shipperName"] = shipperName;
						responseDict["shipperPhone"] = shipperPhone;

						if (customerUser != null)
						{
							var header = "Order Shipped";
							var content = $"Dear customer, your Order (ID: {orderId}) has been shipped and is on its way to you.";

							if (shipperUser != null)
							{
								content = $"Dear customer, your Order (ID: {orderId}) has been assigned to shipper {shipperName} (Phone: {shipperPhone}). Your package is on its way to you!";
							}

							var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}
					}
					else if (model.NewStatus == OrderStatusHelper.Delivered)
					{
						var shipperUser = await _userManager.FindByIdAsync(model.ShipperId);
						var shipperName = shipperUser?.UserName ?? "Unknown";
						var shipperPhone = shipperUser?.PhoneNumber ?? "N/A";

						responseDict["shipperName"] = shipperName;
						responseDict["shipperPhone"] = shipperPhone;

						if (customerUser != null)
						{
							var header = "Order Delivered";
							var content = $"Dear customer, your Order (ID: {orderId}) has been delivered. Please accept it in your Orders page if everything is in good condition.";

							if (shipperUser != null)
							{
								content = $"Dear customer, your Order (ID: {orderId}) has been delivered by {shipperName}. Please accept it in your Orders page if everything is in good condition.";
							}

							var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}
					}
					else if (model.NewStatus == OrderStatusHelper.Completed)
					{
						if (!string.IsNullOrEmpty(order.ShipperID))
						{
							var shipperUser = await _userManager.FindByIdAsync(order.ShipperID);
							if (shipperUser != null)
							{
								var shipperName = shipperUser.UserName ?? "Unknown";
								responseDict["shipperName"] = shipperName;
								responseDict["shipperPhone"] = shipperUser.PhoneNumber ?? "N/A";
							}
						}

						responseDict["shipperId"] = order.ShipperID;

						var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
						var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

						foreach (var adminStaffUser in adminUsers.Union(staffUsers))
						{
							var header = "Order Completed";
							var content = $"Order #{orderId} for customer {customerName} has been confirmed as received. Delivery completed successfully.";

							var notification = await _notificationServices.CreateNotification(adminStaffUser.Id, header, content);

							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(adminStaffUser.Id);
							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}

						if (!string.IsNullOrEmpty(order.ShipperID))
						{
							var shipperUser = await _userManager.FindByIdAsync(order.ShipperID);

							if (shipperUser != null)
							{
								var header = "Order Completed";
								var content = $"Dear Shipper {shipperUser.UserName}, customer {customerName} has confirmed the receipt of Order #{orderId}. Thank you for your service!";

								var notification = await _notificationServices.CreateNotification(order.ShipperID, header, content);

								await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.ShipperID);
								await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
						}
					}
					else if (model.NewStatus == OrderStatusHelper.RefundRequested)
					{
						var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
						var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

						foreach (var adminStaffUser in adminUsers.Union(staffUsers))
						{
							var header = "Refund Request";
							var content = $"Customer {customerName} has requested a refund for order (ID: {orderId})";

							if (!string.IsNullOrEmpty(model.RefundReason))
							{
								content += $"\nReason: {model.RefundReason}";
							}

							var notification = await _notificationServices.CreateNotification(adminStaffUser.Id, header, content);

							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(adminStaffUser.Id);
							await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}

						if (customerUser != null)
						{
							var header = "Refund Request Submitted";
							var content = $"Your refund request for Order (ID: {orderId}) has been submitted. Our team will review it shortly.";

							if (!string.IsNullOrEmpty(model.RefundReason))
							{
								content += $"\nYour reason: {model.RefundReason}";
							}

							var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"ReceiveNotification",
								notification
							);

							var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
							await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
								"UpdateUnreadCount",
								unreadCount
							);
						}

						if (!string.IsNullOrEmpty(order.ShipperID))
						{
							var shipperUser = await _userManager.FindByIdAsync(order.ShipperID);
							if (shipperUser != null)
							{
								var header = "Refund Request for Your Delivery";
								var content = $"A refund has been requested for Order (ID: {orderId}) that you delivered to {customerName}.";

								if (!string.IsNullOrEmpty(model.RefundReason))
								{
									content += $"\nCustomer's reason: {model.RefundReason}";
								}

								var notification = await _notificationServices.CreateNotification(order.ShipperID, header, content);

								await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.ShipperID);
								await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
						}
					}
					else if (model.NewStatus == OrderStatusHelper.Refunded)
					{
						var adminUser = await _userManager.FindByIdAsync(userId);
						var adminName = adminUser?.UserName ?? "Admin";

						if (order.PaymentMethod == "Cash")
						{
							if (!string.IsNullOrEmpty(order.ShipperID))
							{
								var shipperUser = await _userManager.FindByIdAsync(order.ShipperID);
								if (shipperUser != null)
								{
									var header = "Refund Approved for Your Delivery";
									var content = $"A refund request for Order (ID: {orderId}) that you delivered to {customerName} has been approved by Moderator {adminName}.";

									var notification = await _notificationServices.CreateNotification(order.ShipperID, header, content);

									await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
										"ReceiveNotification",
										notification
									);

									var unreadCount = await _notificationServices.GetUnreadCount(order.ShipperID);
									await _notificationHub.Clients.Group($"User_{order.ShipperID}").SendAsync(
										"UpdateUnreadCount",
										unreadCount
									);
								}
							}

							if (customerUser != null)
							{
								var header = "Refund Request Approved";
								var content = $"Your refund request for Order (ID: {orderId}) has been approved by Moderator {adminName}. Please contact our support team to process your cash refund.";

								var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
						}
						else if (order.PaymentMethod == "PayPal")
						{
							if (customerUser != null)
							{
								var header = "Refund Request Approved";
								var content = $"Your refund request for Order (ID: {orderId}) has been approved by Moderator {adminName}. The refund amount of ${order.Total} has been processed and will be credited back to your PayPal account.";

								var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"ReceiveNotification",
									notification
								);

								var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
								await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
									"UpdateUnreadCount",
									unreadCount
								);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error sending order status notification: {ex.Message}");
				}

				return Ok(responseDict);
			}

			return BadRequest(new { message = "Failed to update order status." });
		}

		[HttpPost("custom")]
		public async Task<IActionResult> AddCustomProductOrder([FromBody] OrderCustomProductRequestModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Lấy user từ token
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated");

			// Gọi service, truyền userId riêng
			var order = await _orderServices.AddCustomProductOrderAsync(model, userId);
			var orderViewModel = _mapper.Map<OrderViewModel>(order);

			return CreatedAtAction(nameof(GetOrderById), new { id = orderViewModel.OrderID }, orderViewModel);
		}
	}
}
