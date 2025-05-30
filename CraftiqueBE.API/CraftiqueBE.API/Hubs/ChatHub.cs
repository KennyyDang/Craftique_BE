using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CraftiqueBE.API.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		private readonly IChatServices _chatService;
		private static readonly ConcurrentDictionary<string, string> _userConnections = new();
		private readonly ILogger<ChatHub> _logger;

		public ChatHub(IChatServices chatService, ILogger<ChatHub> logger)
		{
			_chatService = chatService;
			_logger = logger;
		}

		public override async Task OnConnectedAsync()
		{
			try
			{
				var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				_logger.LogInformation($"User attempting to connect. UserId: {userId}");
				_logger.LogInformation($"Connection ID: {Context.ConnectionId}");
				_logger.LogInformation($"User Claims: {string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

				if (!string.IsNullOrEmpty(userId))
				{
					_userConnections.TryAdd(userId, Context.ConnectionId);
					await _chatService.UpdateUserOnlineStatus(userId, true);

					// Notify all clients about the user's online status
					await Clients.All.SendAsync("UserOnline", userId);

					// Get all rooms this user is part of
					var rooms = await _chatService.GetUserChatRooms(userId);
					foreach (var room in rooms)
					{
						await Groups.AddToGroupAsync(Context.ConnectionId, room.ChatRoomID.ToString());
					}
					_logger.LogInformation($"User {userId} successfully connected and joined {rooms.Count} rooms");
				}
				else
				{
					_logger.LogWarning("User connected without authentication");
				}
				await base.OnConnectedAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in OnConnectedAsync");
				throw;
			}
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			try
			{
				var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!string.IsNullOrEmpty(userId))
				{
					_userConnections.TryRemove(userId, out _);
					await _chatService.UpdateUserOnlineStatus(userId, false);
					await Clients.All.SendAsync("UserOffline", userId);
					_logger.LogInformation($"User {userId} disconnected");
				}
				await base.OnDisconnectedAsync(exception);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in OnDisconnectedAsync");
				throw;
			}
		}

		public async Task JoinRoom(int roomId)
		{
			var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("Unauthorized attempt to join room");
				throw new HubException("User not authenticated");
			}

			await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
			_logger.LogInformation($"User {userId} joined room {roomId}");
		}

		public async Task LeaveRoom(int roomId)
		{
			var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("Unauthorized attempt to leave room");
				throw new HubException("User not authenticated");
			}

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
			_logger.LogInformation($"User {userId} left room {roomId}");
		}

		public async Task SendMessage(int roomId, string content)
		{
			try
			{
				var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				_logger.LogInformation($"Attempting to send message. UserId: {userId}, RoomId: {roomId}");

				if (string.IsNullOrEmpty(userId))
				{
					_logger.LogWarning("Authentication failed: User ID is null or empty");
					_logger.LogWarning($"Context.User claims: {string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}: {c.Value}") ?? Array.Empty<string>())}");
					throw new HubException("User not authenticated");
				}

				// Verify user is part of the room
				var rooms = await _chatService.GetUserChatRooms(userId);
				if (!rooms.Any(r => r.ChatRoomID == roomId))
				{
					_logger.LogWarning($"User {userId} attempted to send message to room {roomId} but is not a member");
					throw new HubException("User is not a member of this chat room");
				}

				var message = await _chatService.SendMessage(roomId, userId, content);
				if (message == null)
				{
					_logger.LogError($"Failed to create message for user {userId} in room {roomId}");
					throw new HubException("Failed to create message");
				}

				_logger.LogInformation($"Message sent successfully. MessageId: {message.ChatID}");
				await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in SendMessage");
				throw new HubException($"Failed to send message: {ex.Message}");
			}
		}

		public async Task MarkMessagesAsRead(int roomId)
		{
			try
			{
				var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
				{
					_logger.LogWarning("Authentication failed in MarkMessagesAsRead: User ID is null or empty");
					throw new HubException("User not authenticated");
				}

				await _chatService.MarkMessagesAsRead(roomId, userId);
				await Clients.Group(roomId.ToString()).SendAsync("MessagesRead", roomId, userId);
				_logger.LogInformation($"Messages marked as read for user {userId} in room {roomId}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in MarkMessagesAsRead");
				throw;
			}
		}

		public static List<string> GetConnectedUsers()
		{
			return _userConnections.Keys.ToList();
		}
	}
}
