using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.ViewModels.ChatVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class ChatController : ControllerBase
	{
		private readonly IChatServices _chatService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ChatController> _logger;

		public ChatController(IChatServices chatService, IUnitOfWork unitOfWork, ILogger<ChatController> logger)
		{
			_chatService = chatService;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		private string GetUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		[HttpGet("rooms")]
		public async Task<ActionResult<List<ChatRoomViewModel>>> GetUserChatRooms()
		{
			try
			{
				var userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					_logger.LogWarning("Unauthorized access attempt - no user ID");
					return Unauthorized();
				}

				_logger.LogInformation($"Fetching chat rooms for user: {userId}");
				var rooms = await _chatService.GetUserChatRooms(userId);
				_logger.LogInformation($"Found {rooms.Count} rooms for user {userId}");
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting chat rooms");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		[HttpGet("room/{roomId}")]
		public async Task<ActionResult<ChatRoomViewModel>> GetChatRoom(int roomId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
		{
			try
			{
				var room = await _chatService.GetChatRoom(roomId, pageSize, pageNumber);
				return Ok(room);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPost("room/private")]
		public async Task<ActionResult<ChatRoomViewModel>> CreatePrivateRoom([FromBody] CreatePrivateRoomRequest request)
		{
			try
			{
				var currentUserId = GetUserId();
				if (string.IsNullOrEmpty(currentUserId))
					return Unauthorized("User is not authenticated.");

				if (string.IsNullOrEmpty(request.OtherUserId))
					return BadRequest("Other user ID is required.");

				if (currentUserId == request.OtherUserId)
					return BadRequest("Cannot create a chat room with yourself.");

				_logger.LogInformation($"Creating chat room between {currentUserId} and {request.OtherUserId}");
				var room = await _chatService.CreatePrivateRoom(currentUserId, request.OtherUserId);
				return Ok(room);
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning($"User not found: {ex.Message}");
				return NotFound($"User not found: {ex.Message}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating private chat room");
				return BadRequest($"Error creating chat room: {ex.Message}");
			}
		}

		[HttpGet("users/online")]
		public async Task<ActionResult<List<ChatParticipantViewModel>>> GetOnlineUsers()
		{
			var users = await _chatService.GetOnlineUsers();
			return Ok(users);
		}

		[HttpPost("room/group")]
		public async Task<ActionResult<ChatRoomViewModel>> CreateGroupRoom([FromBody] CreateGroupRoomRequest request)
		{
			var userId = GetUserId();
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			try
			{
				var participants = new List<string> { userId };
				participants.AddRange(request.Participants);

				var room = await _chatService.CreateGroupRoom(request.Name, participants);
				return Ok(room);
			}
			catch (Exception ex)
			{
				return BadRequest($"Error creating group chat: {ex.Message}");
			}
		}

		[HttpGet("room/{roomId}/debug")]
		public async Task<ActionResult> DebugRoom(int roomId)
		{
			try
			{
				var room = await _unitOfWork.ChatRoomRepository
					.GetAllAsync(
						predicate: r => r.ChatRoomID == roomId,
						includes: new Expression<Func<ChatRoom, object>>[]
						{
							r => r.ChatParticipants,
							r => r.ChatMessages
						}
					);

				var debugInfo = new
				{
					RoomFound = room != null,
					MessageCount = room?.FirstOrDefault()?.ChatMessages?.Count ?? 0,
					Messages = room?.FirstOrDefault()?.ChatMessages?.Select(m => new
					{
						m.ChatMessageID,
						m.Content,
						m.CreatedDate,
						m.SenderID
					})
				};

				return Ok(debugInfo);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		public class CreatePrivateRoomRequest
		{
			public string OtherUserId { get; set; }
		}
	}
}
