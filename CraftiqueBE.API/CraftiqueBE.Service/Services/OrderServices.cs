using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using CraftiqueBE.Data.ViewModels.OrderVM;
using CraftiqueBE.Service.Extensions;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class OrderServices : IOrderServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;

		public OrderServices(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_mapper = mapper;
		}

		public async Task<object> GetOrdersAsync(Guid? userId, string? status, TimeModel model, string userRole, Guid currentUserId)
		{
			var query = _unitOfWork.OrderRepository.GetAllQueryable();

			if (userRole == "Customer")
			{
				// Nếu là Customer, chỉ lấy đơn hàng của chính họ
				query = query.Where(o => o.UserID == currentUserId.ToString());
			}
			else if (userRole == "Shipper")
			{
				// Nếu là Shipper, chỉ lấy đơn hàng mà họ đang giao (Shipped) hoặc đã giao thành công (delivered)
				query = query.Where(o => o.ShipperID == currentUserId.ToString());

				if (!string.IsNullOrEmpty(status) && !(status == OrderStatusHelper.Shipped || status == OrderStatusHelper.Delivered))
				{
					throw new ArgumentException($"Shippers can only filter orders by '{OrderStatusHelper.Shipped}' or '{OrderStatusHelper.Delivered}'.");
				}

			}
			else if (userId.HasValue)
			{
				// Nếu là Admin hoặc Staff, có thể lọc theo userId nếu có
				query = query.Where(o => o.UserID == userId.Value.ToString());
			}

			var orders = await query
				.FilterByStatus(status)
				.FilterByYear(model.Year)
				.FilterByQuarter(model.Quarter, model.Year)
				.FilterByMonth(model.Month, model.Year)
				.FilterByDay(model.Day, model.Month, model.Year)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();

			var ordersView = _mapper.Map<List<OrderViewModel>>(orders);
			return new
			{
				IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue || !string.IsNullOrEmpty(status) || userId.HasValue,
				Year = model.Year,
				Quarter = model.Quarter,
				Month = model.Month,
				Day = model.Day,
				Status = status,
				UserId = userRole == "Customer" || userRole == "Shipper" ? currentUserId : userId,
				Orders = ordersView
			};
		}



		public async Task<Order> GetByIdAsync(int id)
		{
			var order = await _unitOfWork.OrderRepository.GetByIdAsync(id, o => o.OrderDetails);
			if (order == null)
				throw new KeyNotFoundException($"Order with ID {id} not found.");

			return order;
		}

		public async Task<Order> AddAsync(OrderModel model)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var order = new Order
				{
					UserID = model.UserID,
					OrderDate = model.OrderDate,
					Address = model.Address,
					PaymentMethod = model.PaymentMethod,
					ShippingMethodID = model.ShippingMethodID,
					OrderStatus = OrderStatusHelper.Pending,
					VoucherID = (model.VoucherID == 0 || model.VoucherID == null) ? null : model.VoucherID,
					Total = model.Total,
					IsDeleted = false
				};

				var result = await _unitOfWork.OrderRepository.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();

				var productIds = model.OrderDetails.Select(od => od.ProductItemID).ToList();
				var products = await _unitOfWork.ProductItemRepository.GetAllAsync(p => productIds.Contains(p.ProductItemID));

				foreach (var item in model.OrderDetails)
				{
					var product = products.FirstOrDefault(p => p.ProductItemID == item.ProductItemID);

					if (product == null)
					{
						throw new KeyNotFoundException($"ProductItem {item.ProductItemID} not found");
					}

					if (product.Quantity < item.Quantity)
					{
						throw new InvalidOperationException($"Insufficient stock for ProductItem {item.ProductItemID}. Available: {product.Quantity}, Requested: {item.Quantity}");
					}

					var orderDetail = new OrderDetail
					{
						OrderID = result.OrderID,
						ProductItemID = item.ProductItemID,
						Price = item.Price,
						Quantity = item.Quantity,
						IsDeleted = false
					};

					await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);

					product.Quantity -= item.Quantity;
					await _unitOfWork.ProductItemRepository.Update(product);
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return result;
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				Console.WriteLine($"Error in AddAsync: {ex.Message} - {ex.InnerException}");
				throw;
			}
		}

		public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, User user, string? shipperId = null)
		{
			var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException("Order not found.");
			}

			if (order.OrderStatus == newStatus)
			{
				throw new InvalidOperationException($"The order is already in '{newStatus}' status.");
			}

			bool isCustomer = await _userManager.IsInRoleAsync(user, RolesHelper.Customer);
			bool isStaff = await _userManager.IsInRoleAsync(user, RolesHelper.Staff);
			bool isAdmin = await _userManager.IsInRoleAsync(user, RolesHelper.Admin);
			bool isShipper = await _userManager.IsInRoleAsync(user, RolesHelper.Shipper);

			// Kiểm tra quyền sở hữu đơn hàng trước khi cập nhật
			if (isCustomer && order.UserID != user.Id.ToString())
			{
				throw new UnauthorizedAccessException("You can only update your own orders.");
			}

			if (isShipper && order.ShipperID != user.Id.ToString())
			{
				throw new UnauthorizedAccessException("You can only update the orders assigned to you.");
			}

			await _unitOfWork.BeginTransactionAsync();
			try
			{
				if (isCustomer)
				{
					await HandleCustomerStatusChange(order, newStatus);
				}
				else if (isStaff || isAdmin)
				{
					await HandleAdminStatusChange(order, newStatus, shipperId);
				}
				else if (isShipper)
				{
					await HandleShipperStatusChange(order, newStatus, shipperId);
				}
				else
				{
					throw new UnauthorizedAccessException("Unauthorized user.");
				}

				await _unitOfWork.OrderRepository.Update(order);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return true;
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				Console.WriteLine($"Error in UpdateOrderStatusAsync: {ex.Message}");
				throw;
			}
		}

		private async Task HandleCustomerStatusChange(Order order, string newStatus)
		{
			if (newStatus == OrderStatusHelper.Cancelled && order.OrderStatus == OrderStatusHelper.Pending)
			{
				order.OrderStatus = OrderStatusHelper.Cancelled;
				await RestoreProductStock(order.OrderID);
			}
			else if (newStatus == OrderStatusHelper.RefundRequested &&
					 (order.OrderStatus == OrderStatusHelper.Delivered || order.OrderStatus == OrderStatusHelper.Completed))
			{
				order.OrderStatus = OrderStatusHelper.RefundRequested;
			}
			else if (newStatus == OrderStatusHelper.Completed && order.OrderStatus == OrderStatusHelper.Delivered)
			{
				order.OrderStatus = OrderStatusHelper.Completed;
			}
			else
			{
				throw new InvalidOperationException("Customers can only cancel pending orders or request refunds for delivered/completed orders.");
			}
		}


		private async Task HandleAdminStatusChange(Order order, string newStatus, string? shipperId)
		{
			var validTransitions = new Dictionary<string, List<string>>
			{
				{ OrderStatusHelper.Pending, new List<string> { OrderStatusHelper.Processing, OrderStatusHelper.Cancelled } },
				{ OrderStatusHelper.Paid, new List<string> { OrderStatusHelper.Processing, OrderStatusHelper.Cancelled } },
				{ OrderStatusHelper.Processing, new List<string> { OrderStatusHelper.Shipped } },
				{ OrderStatusHelper.Shipped, new List<string> { OrderStatusHelper.Delivered } },
				{ OrderStatusHelper.Delivered, new List<string> { OrderStatusHelper.Completed, OrderStatusHelper.RefundRequested } }
				//{ OrderStatusHelper.RefundRequested, new List<string> { OrderStatusHelper.Refunded } }
			};

			if (!validTransitions.ContainsKey(order.OrderStatus) || !validTransitions[order.OrderStatus].Contains(newStatus))
			{
				throw new InvalidOperationException($"Invalid status transition from '{order.OrderStatus}' to '{newStatus}'.");
			}

			if (newStatus == OrderStatusHelper.Cancelled && order.OrderStatus == OrderStatusHelper.Pending)
			{
				order.OrderStatus = OrderStatusHelper.Cancelled;
				await RestoreProductStock(order.OrderID);
			}
			else if (order.OrderStatus == OrderStatusHelper.Processing && newStatus == OrderStatusHelper.Shipped)
			{
				if (string.IsNullOrEmpty(shipperId))
				{
					throw new ArgumentException("A shipper must be assigned before shipping.");
				}

				await AssignShipperToOrder(order, shipperId);
			}
			//else if (order.OrderStatus == OrderStatusHelper.RefundRequested && newStatus == OrderStatusHelper.Refunded)
			//{
			//	if (order.PaymentMethod == "PayPal")
			//	{
			//		//todo
			//		bool refundSuccess = await ProcessPayPalRefund(order);
			//		if (!refundSuccess)
			//		{
			//			throw new InvalidOperationException("PayPal refund failed.");
			//		}
			//	}

			//	await RestoreProductStock(order.OrderID);
			//}

			order.OrderStatus = newStatus;
		}

		private async Task HandleShipperStatusChange(Order order, string newStatus, string? shipperId)
		{
			if (newStatus == OrderStatusHelper.Delivered && order.OrderStatus == OrderStatusHelper.Shipped)
			{
				if (order.ShipperID != shipperId)
				{
					throw new UnauthorizedAccessException("You are not assigned to this order.");
				}
				order.OrderStatus = OrderStatusHelper.Delivered;
			}

			else
			{
				throw new InvalidOperationException("Shipper can only update 'Shipped' orders to 'Delivered'.");
			}
		}

		private async Task AssignShipperToOrder(Order order, string shipperId)
		{
			var shipper = await _userManager.FindByIdAsync(shipperId);
			if (shipper == null || !await _userManager.IsInRoleAsync(shipper, RolesHelper.Shipper))
			{
				throw new ArgumentException("Invalid shipper ID or user is not a shipper.");
			}

			int activeDeliveries = await _unitOfWork.OrderRepository.GetAllQueryable()
				.CountAsync(o => o.ShipperID == shipperId && o.OrderStatus == OrderStatusHelper.Shipped);

			if (activeDeliveries >= 3)
			{
				throw new InvalidOperationException("This shipper is already handling 3 deliveries.");
			}

			order.ShipperID = shipperId;
		}


		private async Task RestoreProductStock(int orderId)
		{
			var orderDetails = await _unitOfWork.OrderDetailRepository
				 .GetAllQueryable()
				 .Where(od => od.OrderID == orderId)
				 .AsNoTracking()
				 .ToListAsync();

			var productIds = orderDetails.Select(od => od.ProductItemID).Distinct().ToList();

			var products = await _unitOfWork.ProductItemRepository
				.GetAllQueryable()
				.Where(p => productIds.Contains(p.ProductItemID))
				.ToListAsync();


			foreach (var item in orderDetails)
			{
				var product = products.FirstOrDefault(p => p.ProductItemID == item.ProductItemID);
				if (product != null)
				{
					product.Quantity += item.Quantity;
					await _unitOfWork.ProductItemRepository.Update(product);
				}
			}

			await _unitOfWork.SaveChangesAsync();

		}

		//todo
		//private async Task<bool> ProcessPayPalRefund(Order order)
		//{
		//	try
		//	{
		//		//var paypalService = new PayPalService(); // Giả sử có class tích hợp PayPal
		//		//bool refundSuccess = await paypalService.RefundPayment(order.PaymentTransactionId, order.Total);

		//		//return refundSuccess;
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine($"PayPal refund error: {ex.Message}");
		//		return false;
		//	}
		//}

	}
}
