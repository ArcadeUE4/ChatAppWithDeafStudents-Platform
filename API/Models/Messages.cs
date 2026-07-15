namespace ChatAppWithDeafStudents.API.Models
{
    /// <summary>
    /// Represents a message entity within a chat session.
    /// </summary>
    public class Messages
    {
        /// <summary>
        /// Gets or sets the unique identifier 
        /// of the message. Defaults to a new GUID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the unique identifier 
        /// of the chat to which the message belongs.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Chats"/> entity.
        /// </summary>
        public Chats Chat { get; set; } = new();

        /// <summary>
        /// Gets or sets the unique identifier 
        /// of the user who sent the message.
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// Gets or sets the associated 
        /// sender <see cref="Users"/> entity.
        /// </summary>
        public Users Sender { get; set; } = new();

        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// the message content was converted from voice.
        /// </summary>
        public bool IsVoiceToText { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the message was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether the message 
        /// has been read by the recipient.
        /// </summary>
        public bool IsRead { get; set; } = false;
    }
}
