using AndroidX.AppCompat.View.Menu;

namespace ChatAppWithDeafStudents.Client.Models
{
    public class Users
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatMembers> ChatMembers { get; set; } = new List<ChatMembers>();
        public ICollection<Messages> Messages { get; set; } = new List<Messages>();
    }
   
}
