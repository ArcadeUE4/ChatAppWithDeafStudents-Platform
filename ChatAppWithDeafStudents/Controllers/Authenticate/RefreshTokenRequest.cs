using System.ComponentModel.DataAnnotations;

namespace ChatAppWithDeafStudents.API.Controllers.Authenticate
{
    /// <summary>
    /// Represents a request to refresh an access token.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        [MinLength(10, ErrorMessage = "Refresh token must be at least 10 characters long")]
        [MaxLength(1024, ErrorMessage = "Refresh token must not exceed 1024 characters")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
