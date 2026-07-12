using Newtonsoft.Json;

namespace ChatAppWithDeafStudents.Client.Services.Authentication
{
    public class AuthenticateResponse : BaseResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        [JsonProperty("AccessToken")]
        public string Token { get; set; } = string.Empty;

        [JsonProperty("RefreshToken")]
        public string RefreshTokenValue { get; set; } = string.Empty;
    }
}
