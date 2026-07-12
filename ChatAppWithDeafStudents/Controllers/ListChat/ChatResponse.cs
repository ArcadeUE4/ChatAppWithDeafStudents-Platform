namespace ChatAppWithDeafStudents.API.Controllers.ListChat
{
    /// <summary>
    /// Represents the response model for chat information.
    /// </summary>
    public class ChatResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the chat.
        /// </summary>
        public Guid Id { get; set;  }

        /// <summary>
        /// Gets or sets the title or name of the chat.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the chat is a group chat.
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the chat was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
