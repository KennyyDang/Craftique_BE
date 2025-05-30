using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.OrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IOrderServices
	{
		Task<Order> AddAsync(OrderModel model);
		Task<Order> GetByIdAsync(int id);
		Task<object> GetOrdersAsync(Guid? userId, string? status, TimeModel model, string userRole, Guid currentUserId);
		Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, User user, string? shipperId = null);
	}
}
