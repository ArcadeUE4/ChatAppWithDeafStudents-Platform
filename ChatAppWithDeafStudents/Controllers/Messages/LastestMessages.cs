namespace ChatAppWithDeafStudents.API.Controllers.Messages
{
    /// <summary>
    /// Represents the latest message details in a chat.
    /// </summary>
    public class LastestMessages
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the chat.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who sent the message.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the message was sent.
        /// </summary>
        public DateTime SendDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message has been read.
        /// </summary>
        public bool IsRead { get; set; }
    }
}
