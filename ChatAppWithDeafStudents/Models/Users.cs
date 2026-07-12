namespace ChatAppWithDeafStudents.API.Models
{

    /// <summary>
    /// Represents a user entity within the system.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// Defaults to a new GUID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed password for the user.
        /// BCrypt includes the salt within the hash.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role identifier 
        /// for the user's permissions.
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// Gets or sets the date and time when 
        /// the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of chat
        /// memberships associated with the user.
        /// </summary>
        public ICollection<ChatMembers> ChatMembers { get; set; } = new List<ChatMembers>();

        /// <summary>
        /// Gets or sets the collection of 
        /// messages sent by the user.
        /// </summary>
        public ICollection<Messages> Messages { get; set; } = new List<Messages>();
    }

}
