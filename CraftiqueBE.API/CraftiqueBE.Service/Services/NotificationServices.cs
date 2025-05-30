using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class NotificationServices : INotificationServices
	{
		private readonly IUnitOfWork _unitOfWork;

		public NotificationServices(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Notification> CreateNotification(string userId, string header, string content)
		{
			var notification = new Notification
			{
				UserID = userId,
				Header = header,
				Content = content,
				IsRead = false,
				IsDeleted = false,
				CreatedDate = DateTime.UtcNow
			};

			await _unitOfWork.NotificationRepository.AddAsync(notification);
			await _unitOfWork.SaveChangesAsync();

			return notification;
		}

		public async Task<List<Notification>> GetUserNotifications(string userId)
		{
			var notifications = await _unitOfWork.NotificationRepository.GetAllAsync(
				n => n.UserID == userId && !n.IsDeleted
			);

			return notifications
				.OrderByDescending(n => n.CreatedDate)
				.ToList();
		}

		public async Task<bool> MarkAsRead(int notificationId)
		{
			var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
			if (notification == null) return false;

			notification.IsRead = true;
			await _unitOfWork.NotificationRepository.Update(notification);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteNotification(int notificationId)
		{
			var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
			if (notification == null) return false;

			notification.IsDeleted = true;
			await _unitOfWork.NotificationRepository.Update(notification);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<int> GetUnreadCount(string userId)
		{
			var notifications = await _unitOfWork.NotificationRepository.GetAllAsync(
				n => n.UserID == userId && !n.IsDeleted && !n.IsRead
			);

			return notifications.Count();
		}
	}
}
