using ChatAppWithDeafStudents.Client.Models;

namespace ChatAppWithDeafStudents.Client.Services.Message
{
    public class MessageInitializeRequest : BaseResponse
    {
        public Guid UserId { get; set; }

        public Guid ChatId { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool IsVoiceToText { get; set; } = false;
    }

    public class MessageInitializeResponse : BaseResponse
    {
        public Guid UserId { get; set; }

        public Guid ChatId { get; set; }

        public Users FriendInfo { get; set; } = new();

        public Chats ChatInfo { get; set; } = new();

        public IEnumerable<Messages> Messages { get; set; } 
            = new List<Messages>()!;
    }
}
