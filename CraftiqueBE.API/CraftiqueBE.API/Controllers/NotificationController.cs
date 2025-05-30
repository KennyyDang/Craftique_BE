using CraftiqueBE.Data.Entities;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationServices _notificationService;
		private readonly ILogger<NotificationController> _logger;

		public NotificationController(INotificationServices notificationService, ILogger<NotificationController> logger)
		{
			_notificationService = notificationService;
			_logger = logger;
		}

		private string GetUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		[HttpGet]
		public async Task<ActionResult<List<Notification>>> GetUserNotifications()
		{
			try
			{
				var userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized();
				}

				var notifications = await _notificationService.GetUserNotifications(userId);
				return Ok(notifications);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user notifications");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("unread-count")]
		public async Task<ActionResult<int>> GetUnreadCount()
		{
			try
			{
				var userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized();
				}

				var count = await _notificationService.GetUnreadCount(userId);
				return Ok(count);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting unread count");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}/read")]
		public async Task<ActionResult> MarkAsRead(int id)
		{
			try
			{
				var success = await _notificationService.MarkAsRead(id);
				if (!success)
				{
					return NotFound();
				}

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error marking notification as read");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteNotification(int id)
		{
			try
			{
				var success = await _notificationService.DeleteNotification(id);
				if (!success)
				{
					return NotFound();
				}

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting notification");
				return StatusCode(500, "Internal server error");
			}
		}
	}
}
