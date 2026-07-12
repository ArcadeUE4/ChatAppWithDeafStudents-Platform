using ChatAppWithDeafStudents.API.Models;

namespace ChatAppWithDeafStudents.API.Functions.Chat
{
    /// <summary>
    /// Defines the contract for chat-related operations, including retrieval of chat details,
    /// members, user information, and recent message history.
    /// </summary>
    /// <remarks>
    /// This interface provides operations for:
    /// <list type="bullet">
    /// <item><description>Retrieving user chats and chat membership information</description></item>
    /// <item><description>Fetching user profile details</description></item>
    /// <item><description>Getting the most recent messages from user's chats</description></item>
    /// </list>
    /// </remarks>
    public interface IChatFunction
    {
        /// <summary>
        /// Retrieves all chats associated with a specific user.
        /// </summary>
        /// <remarks>
        /// Returns both direct chats and group chats that the user is a member of.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of <see cref="Chats"/> entities.</returns>
        Task<IEnumerable<Chats>> GetUserChatsAsync(Guid userId);

        /// <summary>
        /// Retrieves a list of chat memberships for a specific user,
        /// including associated chat metadata.
        /// </summary>
        /// <remarks>
        /// Returns dynamic objects containing both user and chat membership information
        /// for efficient client-side data binding.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing a collection of dynamic chat membership objects.</returns>
        Task<IEnumerable<dynamic>> GetUserChatMembersAsync(Guid userId);

        /// <summary>
        /// Fetches user profile information based on the provided user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing the <see cref="Users"/> entity with user profile information.</returns>
        Task<Users> GetUserInfoAsync(Guid userId);

        /// <summary>
        /// Retrieves the most recent message from every chat the user is a member of.
        /// </summary>
        /// <remarks>
        /// Returns dynamic objects containing the latest message and associated metadata
        /// for efficient client-side chat list display.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing a collection of dynamic objects representing the latest messages from each chat.</returns>
        Task<IEnumerable<dynamic>> GetLastestMessagesAsync(Guid userId);
    }
}
