using ChatAppWithDeafStudents.API.Functions.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatAppWithDeafStudents.API.Controllers
{
    /// <summary>
    /// Handles real-time communication for chat functionality using SignalR.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageFunction _messageFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHub"/> class.
        /// </summary>
        /// <param name="messageFunction">The service used to
        /// persist and process chat messages.</param>
        public ChatHub(IMessageFunction messageFunction)
        {
            _messageFunction = messageFunction;
        }

        /// <summary>
        /// Sends a message to a specific chat group and persists it to the database.
        /// </summary>
        /// <param name="chatId">The unique identifier of the chat group.</param>
        /// <param name="content">The text content of the message.</param>
        /// <param name="isVoiceToText">Indicates if the message was converted from voice.</param>
        /// <returns>A task representing the asynchronous send operation.</returns>
        /// <remarks>
        /// SECURITY: The sender ID is extracted from the authenticated JWT token, not from client input.
        /// This prevents clients from spoofing messages as other users.
        /// </remarks>
        public async Task SendMessage(Guid chatId, string content, bool isVoiceToText)
        {
            // Get the authenticated user's ID from JWT token
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
                throw new HubException("User is not authenticated or has invalid user ID");
            }

            // Use the authenticated user's ID instead of allowing client to specify senderId
            var messageId = await _messageFunction.AddMessage(userGuid, chatId, content, isVoiceToText);

            // Send full message information to all clients in the group
            await Clients.Group(chatId.ToString()).SendAsync(
                "ReceiveMessage", 
                messageId,
                userGuid, 
                content, 
                isVoiceToText,
                DateTime.UtcNow);
        }

        /// <summary>
        /// Adds the current client connection to a specific chat group.
        /// </summary>
        /// <param name="chatId">The unique identifier of the chat to join.</param>
        /// <returns>A task representing the asynchronous group join operation.</returns>
        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
