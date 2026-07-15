namespace ChatAppWithDeafStudents.API.Models
{
    /// <summary>
    /// Represents a chat entity in the system, which can be either a direct conversation or a group chat.
    /// </summary>
    public class Chats
    {
        /// <summary>
        /// Gets or sets the unique identifier of the chat. Defaults to a new GUID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the title or name of the chat.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this is a group chat.
        /// </summary>
        public bool IsGroup { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the chat was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of members associated with this chat.
        /// </summary>
        public ICollection<ChatMembers> ChatMembers { get; set; } = new List<ChatMembers>();

        /// <summary>
        /// Gets or sets the collection of messages sent within this chat.
        /// </summary>
        public ICollection<Messages> Messages { get; set; } = new List<Messages>();
    }
}
