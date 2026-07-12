namespace ChatAppWithDeafStudents.Client.Services.Authentication
{
    public class AuthenticateRequest
    {
        public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
    }
}
