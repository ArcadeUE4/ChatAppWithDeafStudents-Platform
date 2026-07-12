using ChatAppWithDeafStudents.API.Functions.Chat;
using ChatAppWithDeafStudents.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ChatAppWithDeafStudents.API.Controllers.ListChat
{
    /// <summary>
    /// Handles chat-related operations, including initialization and message retrieval.
    /// Requires authentication for all endpoints.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for:
    /// <list type="bullet">
    /// <item><description>Initializing the chat interface with user info and chat members</description></item>
    /// <item><description>Retrieving chat details for a specific chat</description></item>
    /// </list>
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatFunction _chatFunction;
        private readonly ILogger<ChatController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatFunction">The chat service implementation.</param>
        /// <param name="logger">The logger instance.</param>
        public ChatController(IChatFunction chatFunction, ILogger<ChatController> logger)
        {
            _chatFunction = chatFunction;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the chat interface by fetching user info, chat members, 
        /// and recent messages for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This method verifies that the requesting user is authorized to initialize 
        /// their own chat interface before returning their data.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user requesting initialization.</param>
        /// <returns>
        /// <see cref="OkResult"/> with user information, chat members, and the latest messages.
        /// <see cref="ForbidResult"/> if the requesting user does not match the userId parameter.
        /// <see cref="OkResult"/> with error status if an exception occurs.
        /// </returns>
        /// <response code="200">Initialization successful</response>
        /// <response code="403">User is not authorized to initialize this chat</response>
        /// <response code="500">Internal server error during initialization</response>
        [HttpPost("Initialize")]
        public async Task<IActionResult> Initialize([FromBody] Guid userId)
        {
            // Validate that the requesting user matches the userId
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(requestingUserId) || requestingUserId != userId.ToString())
            {
                return Forbid();
            }

            try
            {
                _logger.LogInformation($"ChatController.Initialize called for userId: {userId}");

                var userInfo = await _chatFunction.GetUserInfoAsync(userId);
                _logger.LogInformation($"Got userInfo: {userInfo?.FullName}");

                var chatMembers = await _chatFunction.GetUserChatMembersAsync(userId);
                _logger.LogInformation($"Got {chatMembers?.Count() ?? 0} chat members");

                var lastestMessages = await _chatFunction.GetLastestMessagesAsync(userId);
                _logger.LogInformation($"Got {lastestMessages?.Count() ?? 0} latest messages");

                return Ok(new
                {
                    Users = userInfo,
                    ChatMembers = chatMembers,
                    LastestMessages = lastestMessages,
                    StatusCode = 200,
                    StatusMessage = "Success"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ChatController.Initialize for userId: {userId}");
                return Ok(new
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific chat.
        /// </summary>
        /// <remarks>
        /// Returns all chats associated with the specified chat identifier, 
        /// filtered to the authenticated user's permissions.
        /// </remarks>
        /// <param name="chatId">The unique identifier of the chat to retrieve.</param>
        /// <returns>
        /// <see cref="OkResult"/> containing the requested chat data.
        /// </returns>
        /// <response code="200">Chat data retrieved successfully</response>
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChats(Guid chatId)
        {
            var chats = await _chatFunction.GetUserChatsAsync(chatId);
            return Ok(chats);
        }
    }
}
