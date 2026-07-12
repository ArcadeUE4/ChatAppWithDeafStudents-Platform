namespace ChatAppWithDeafStudents.Client.Models
{
    public class ChatMembers
    {
        public Guid ChatId { get; set; }
        public Chats Chat { get; set; } = new Chats();
        public Guid UserId { get; set; }
        public Users User { get; set; } = new Users();

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public LastestMessages LastestMessage { get; set; } = new LastestMessages();
    }
}
