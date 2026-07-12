using ChatAppWithDeafStudents.API.Models;

namespace ChatAppWithDeafStudents.API.Controllers.Messages
{
    /// <summary>
    /// Represents the response model containing the initial state of a chat session.
    /// </summary>
    public class MessageInitializeResponse
    {
        /// <summary>
        /// Gets or sets the HTTP status code of the operation.
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Gets or sets the status message providing additional context about the operation result.
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the list of messages retrieved for the chat.
        /// </summary>
        public List<Models.Messages> Messages { get; set; } = new();
        
        /// <summary>
        /// Gets or sets the information about the chat itself.
        /// </summary>
        public Chats ChatInfo { get; set; } = new();
        
        /// <summary>
        /// Gets or sets the information about the other participant in the chat.
        /// </summary>
        public Users FriendInfo { get; set; } = new();
    }
}
