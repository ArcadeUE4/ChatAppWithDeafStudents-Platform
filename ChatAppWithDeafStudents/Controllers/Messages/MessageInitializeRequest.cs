using System.ComponentModel.DataAnnotations;
using ChatAppWithDeafStudents.API.Configuration;

namespace ChatAppWithDeafStudents.API.Controllers.Messages
{
    /// <summary>
    /// Represents the request model for initializing a chat session or sending a message.
    /// </summary>
    public class MessageInitializeRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user initiating the request.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the chat.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        [StringLength(4096, ErrorMessage = "Message content must not exceed 4096 characters")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the message content was generated via voice-to-text.
        /// </summary>
        public bool IsVoiceToText { get; set; }
    }
}
