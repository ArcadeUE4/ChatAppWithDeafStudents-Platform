using ChatAppWithDeafStudents.API.Functions.Message;
using ChatAppWithDeafStudents.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatAppWithDeafStudents.API.Controllers.Messages
{
    /// <summary>
    /// Handles message-related operations such as retrieving history, sending messages, 
    /// and initializing chat views. Requires authentication for all endpoints.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for:
    /// <list type="bullet">
    /// <item><description>Retrieving paginated message history for a chat</description></item>
    /// <item><description>Retrieving full message history for a chat</description></item>
    /// <item><description>Initializing a chat view with messages and participant information</description></item>
    /// <item><description>Sending new messages to a chat</description></item>
    /// </list>
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/Message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageFunction _messageFunction;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MessageController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="messageFunction">The service for message-related business logic.</param>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public MessageController(
            IMessageFunction messageFunction,
            ApplicationDbContext context,
            ILogger<MessageController> logger)
        {
            _messageFunction = messageFunction;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Initializes a chat view by retrieving messages, chat details, and participant information.
        /// </summary>
        /// <remarks>
        /// This endpoint prepares a chat view for the client by returning:
        /// <list type="bullet">
        /// <item><description>All messages in the chat</description></item>
        /// <item><description>Chat metadata and settings</description></item>
        /// <item><description>For direct chats: the friend/participant information</description></item>
        /// </list>
        /// The requesting user must be a member of the chat.
        /// Circular references are removed from all objects to prevent serialization issues.
        /// </remarks>
        /// <param name="request">The request containing user and chat identifiers.</param>
        /// <returns>
        /// <see cref="OkResult"/> containing <see cref="MessageInitializeResponse"/> with messages, chat info, and friend info.
        /// <see cref="ForbidResult"/> if the user is not a member of the chat.
        /// <see cref="BadRequestResult"/> if the model is invalid or an error occurs.
        /// </returns>
        /// <response code="200">Chat initialization successful</response>
        /// <response code="400">Invalid request or internal error</response>
        /// <response code="403">User is not a member of this chat</response>
         [HttpPost("Initialize")]
        public async Task<IActionResult> Initialize([FromBody] MessageInitializeRequest request)
        {
            // Validate the incoming request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Initialize request - ChatId: {request.ChatId}, UserId: {request.UserId}");

                // Validate that ChatId is provided
                if (request.ChatId == Guid.Empty)
                {
                    _logger.LogWarning($"ChatId is empty");
                    return BadRequest(new { StatusCode = 400, StatusMessage = "ChatId is required" });
                }

                // Validate that user is authenticated
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning($"No authenticated user found");
                    return Forbid();
                }

                _logger.LogInformation($"User authenticated: {userId}");

                // Validate that the user ID format is valid
                if (!Guid.TryParse(userId, out var userIdGuid))
                {
                    _logger.LogWarning($"Invalid user ID format: {userId}");
                    return Forbid();
                }

                // Check if the chat exists
                var chatExists = await _context.Chats.AsNoTracking().AnyAsync(c => c.Id == request.ChatId);
                if (!chatExists)
                {
                    _logger.LogWarning($"Chat not found: {request.ChatId}");
                    return BadRequest(new { StatusCode = 400, StatusMessage = "Chat not found" });
                }

                _logger.LogInformation($"Chat found: {request.ChatId}");

                // Validate that the user is a member of the chat
                var isMember = await _context.ChatMembers
                    .AsNoTracking()
                    .AnyAsync(cm => cm.ChatId == request.ChatId && cm.UserId == userIdGuid);

                if (!isMember)
                {
                    _logger.LogWarning($"User {userIdGuid} is NOT a member of chat {request.ChatId}");
                    return Forbid();
                }

                _logger.LogInformation($"User {userIdGuid} is member of chat {request.ChatId}");

                // Get messages without tracking and without circular references
                var messages = await _context.Messages
                    .AsNoTracking()
                    .Where(m => m.ChatId == request.ChatId)
                    .Include(m => m.Sender)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation($"Found {messages.Count} messages for chat {request.ChatId}");

                // Get chat information
                var chat = await _context.Chats.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ChatId);
                _logger.LogInformation($"Chat info: {(chat != null ? chat.Title : "NOT FOUND")}");

                // Get friend information
                Users? friendInfo = null;
                if (chat != null && !chat.IsGroup)
                {
                    friendInfo = await _context.ChatMembers
                        .AsNoTracking()
                        .Where(cm => cm.ChatId == request.ChatId && cm.UserId != userIdGuid)
                        .Include(cm => cm.User)
                        .Select(cm => cm.User)
                        .FirstOrDefaultAsync();

                    _logger.LogInformation($"Friend info: {(friendInfo != null ? friendInfo.FullName : "NOT FOUND")}");
                }

                // Clear circular references in messages
                foreach (var msg in messages)
                {
                    if (msg.Sender != null)
                    {
                        msg.Sender.ChatMembers.Clear();
                        msg.Sender.Messages.Clear();
                    }
                    msg.Chat = null!;
                }

                // Clear circular references in chat
                if (chat != null)
                {
                    chat.ChatMembers.Clear();
                    chat.Messages.Clear();
                }

                // Clear circular references in friend info
                if (friendInfo != null)
                {
                    friendInfo.ChatMembers.Clear();
                    friendInfo.Messages.Clear();
                }

                _logger.LogInformation($"Returning {messages.Count} messages");

                return Ok(new MessageInitializeResponse
                {
                    StatusCode = 200,
                    StatusMessage = "OK",
                    Messages = messages,
                    ChatInfo = chat ?? new Chats(),
                    FriendInfo = friendInfo ?? new Users()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initializing chat {request.ChatId}");
                return BadRequest(new
                {
                    StatusCode = 400,
                    StatusMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Sends a new message to a specified chat.
        /// </summary>
        /// <remarks>
        /// This endpoint allows authenticated users to send messages to a chat. 
        /// The requesting user must:
        /// 1. Match the UserId in the request (authentication check)
        /// 2. Be a member of the specified chat (authorization check)
        /// Voice-to-text flag can be set to indicate the message was generated from voice input.
        /// </remarks>
        /// <param name="request">The request containing message content, chat ID, and sender information.</param>
        /// <returns>
        /// <see cref="OkResult"/> containing the unique identifier of the created message.
        /// <see cref="ForbidResult"/> if the requesting user is not a member of the chat.
        /// <see cref="BadRequestResult"/> if validation fails or an expected error occurs.
        /// <see cref="StatusCodeResult"/> with status 500 if an unexpected error occurs.
        /// </returns>
        /// <response code="200">Message sent successfully</response>
        /// <response code="400">Invalid request data or validation error</response>
        /// <response code="403">User is not a member of this chat</response>
        /// <response code="500">Internal server error while sending message</response>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageInitializeRequest request)
        {
            try
            {
                // Validate that the requesting user matches the UserId in request
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || userId != request.UserId.ToString())
                {
                    _logger.LogWarning($"Unauthorized send attempt: expected user {request.UserId}, got {userId}");
                    return Forbid();
                }

                // Validate request content
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        StatusMessage = "Message content cannot be empty."
                    });
                }

                // Validate content length (max 5000 characters)
                if (request.Content.Length > 5000)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        StatusMessage = "Message content exceeds maximum length of 5000 characters."
                    });
                }

                var messageId = await _messageFunction.AddMessage(
                    request.UserId,
                    request.ChatId,
                    request.Content,
                    request.IsVoiceToText
                );

                _logger.LogInformation($"Message sent successfully - User: {request.UserId}, Chat: {request.ChatId}");
                return Ok(new { MessageId = messageId });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error sending message: {ex.Message}");
                return BadRequest(new
                {
                    StatusCode = 400,
                    StatusMessage = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid operation sending message: {ex.Message}");
                return BadRequest(new
                {
                    StatusCode = 400,
                    StatusMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error sending message for user {request.UserId}");
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred while sending the message."
                });
            }
        }
    }

}
