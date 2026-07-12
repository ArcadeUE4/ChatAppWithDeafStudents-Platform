namespace ChatAppWithDeafStudents.API.Models
{
    /// <summary>
    /// Represents the membership association between a user and a chat.
    /// </summary>
    public class ChatMembers
    {
        /// <summary>
        /// Gets or sets the unique identifier of the chat.
        /// </summary>
        public Guid ChatId { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Chats"/> entity.
        /// </summary>
        public Chats Chat { get; set; } = new();

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Users"/> entity.
        /// </summary>
        public Users User { get; set; } = new();

        /// <summary>
        /// Gets or sets the date and time when the user joined the chat.
        /// </summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
