namespace ChatAppWithDeafStudents.Client.Models
{
    public class Messages
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChatId { get; set; }
        public Chats Chat { get; set; } = new();
        public Guid SenderId { get; set; }
        public Users Sender { get; set; } = new();
        public string Content { get; set; } = string.Empty;
        public bool IsVoiceToText { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
