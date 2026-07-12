namespace ChatAppWithDeafStudents.Client.Models
{
    public class LastestMessages
    {
        public Guid Id { get; set; }           
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }       
        public string Content { get; set; } = string.Empty; 
        public DateTime SendDateTime { get; set; } 
        public bool IsRead { get; set; }
    }
}
