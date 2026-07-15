using System.ComponentModel.DataAnnotations;

namespace ChatAppWithDeafStudents.API.Controllers.Authenticate
{
    /// <summary>
    /// Request model for user authentication.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string password { get; set; } = string.Empty;
    }
}
