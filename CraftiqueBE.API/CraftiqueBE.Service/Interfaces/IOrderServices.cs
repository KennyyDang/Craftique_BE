using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using CraftiqueBE.Data.ViewModels.OrderVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IOrderServices
	{
		Task<Order> AddAsync(OrderModel model, string userId);
		Task<OrderViewModel> GetByIdAsync(int id);
		Task<object> GetOrdersAsync(Guid? userId, string? status, TimeModel model, string userRole, Guid currentUserId);
		Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, User user, string? shipperId = null);
		Task<OrderStatisticsViewModel> GetOrderStatisticsAsync();
		Task<Order> AddCustomProductOrderAsync(OrderCustomProductRequestModel model, String userId);
	}
}
