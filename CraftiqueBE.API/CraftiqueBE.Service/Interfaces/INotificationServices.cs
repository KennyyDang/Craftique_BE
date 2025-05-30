using CraftiqueBE.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface INotificationServices
	{
		Task<Notification> CreateNotification(string userId, string header, string content);
		Task<List<Notification>> GetUserNotifications(string userId);
		Task<bool> MarkAsRead(int notificationId);
		Task<bool> DeleteNotification(int notificationId);
		Task<int> GetUnreadCount(string userId);
	}
}
