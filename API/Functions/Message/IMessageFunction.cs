using ChatAppWithDeafStudents.API.Controllers.Messages;
using ChatAppWithDeafStudents.API.Models;

namespace ChatAppWithDeafStudents.API.Functions.Message
{
    /// <summary>
    /// Defines the contract for message-related operations, including retrieval of message history,
    /// fetching the latest messages, and adding new messages to the system.
    /// </summary>
    /// <remarks>
    /// This interface provides operations for:
    /// <list type="bullet">
    /// <item><description>Retrieving message history from chats with pagination support</description></item>
    /// <item><description>Fetching the latest messages from user's chats</description></item>
    /// <item><description>Persisting new messages to the database</description></item>
    /// </list>
    /// All operations are asynchronous to support responsive user interfaces.
    /// </remarks>
    public interface IMessageFunction
    {
        /// <summary>
        /// Retrieves the latest messages associated with a specific user from all their chats.
        /// </summary>
        /// <remarks>
        /// Returns the most recent message from each chat the user is a member of,
        /// useful for populating a chat list view on the client.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing a collection of <see cref="LastestMessages"/> objects.</returns>
        Task<IEnumerable<LastestMessages>> GetLastestMessages(Guid userId);

        /// <summary>
        /// Retrieves the complete message history for a specific chat.
        /// </summary>
        /// <remarks>
        /// Returns all messages in the chat without pagination, ordered chronologically from oldest to newest.
        /// For large chats, consider using <see cref="GetMessagesPaginated"/> instead.
        /// </remarks>
        /// <param name="senderId">The unique identifier of the user requesting the messages. Used for authorization.</param>
        /// <param name="chatId">The unique identifier of the chat.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of <see cref="Messages"/>.</returns>
        Task<IEnumerable<Messages>> GetMessages(Guid senderId, Guid chatId);

        /// <summary>
        /// Retrieves paginated message history for a specific chat context.
        /// </summary>
        /// <remarks>
        /// Messages are returned in chronological order with configurable page size for efficient loading.
        /// Recommended for chats with large message histories to avoid excessive data transfer.
        /// </remarks>
        /// <param name="senderId">The unique identifier of the user requesting the messages. Used for authorization.</param>
        /// <param name="chatId">The unique identifier of the chat.</param>
        /// <param name="pageNumber">The page number for pagination (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of messages per page. Default is 50.</param>
        /// <returns>A task representing the asynchronous operation, containing a <see cref="PaginatedResponse{Messages}"/> with total count and page metadata.</returns>
        Task<PaginatedResponse<Messages>> GetMessagesPaginated(Guid senderId, Guid chatId, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Adds a new message to the chat and persists it to the data store.
        /// </summary>
        /// <remarks>
        /// This method handles message persistence and can be triggered both from HTTP endpoints
        /// and from SignalR connections for real-time messaging.
        /// </remarks>
        /// <param name="senderId">The unique identifier of the user sending the message.</param>
        /// <param name="chatId">The unique identifier of the chat.</param>
        /// <param name="content">The text content of the message.</param>
        /// <param name="isVoiceToText">Indicates if the message was generated via voice-to-text conversion.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing the unique identifier of the newly created message.</returns>
        Task<Guid> AddMessage(Guid senderId, Guid chatId, string content, bool isVoiceToText);

    }
}
