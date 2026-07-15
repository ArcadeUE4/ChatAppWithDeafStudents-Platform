namespace ChatAppWithDeafStudents.API.Controllers.Authenticate
{
    /// <summary>
    /// Response model for authentication endpoint.
    /// </summary>
    public class AuthenticateTokenResponse
    {
        /// <summary>
        /// User's unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Short-lived JWT access token (15 minutes).
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Long-lived refresh token (7 days).
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Token type (always "Bearer").
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Access token expiration time in seconds (900 = 15 minutes).
        /// </summary>
        public int ExpiresIn { get; set; } = 900;

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Status message.
        /// </summary>
        public string StatusMessage { get; set; } = "Authentication successful";
    }
}
