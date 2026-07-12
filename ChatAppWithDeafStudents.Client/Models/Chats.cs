namespace ChatAppWithDeafStudents.Client.Models
{
    public class Chats
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public bool IsGroup { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatMembers> ChatMembers { get; set; } = new List<ChatMembers>();
        public ICollection<Messages> Messages { get; set; } = new List<Messages>();
    }
}
