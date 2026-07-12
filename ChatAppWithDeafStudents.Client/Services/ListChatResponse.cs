using ChatAppWithDeafStudents.Client.Models;

namespace ChatAppWithDeafStudents.Client.Services
{
    public class ListChatResponse : BaseResponse
    {
        public Users Users { get; set; } = new();

        public IEnumerable<ChatMembers> ChatMembers { get; set; } = new List<ChatMembers>();

       public IEnumerable<LastestMessages> LastestMessages { get; set; } = new List<LastestMessages>();
    }
}
