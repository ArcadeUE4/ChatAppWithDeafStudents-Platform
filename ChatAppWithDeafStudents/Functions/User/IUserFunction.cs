using ChatAppWithDeafStudents.API.Models;

namespace ChatAppWithDeafStudents.API.Functions.User
{
    /// <summary>
    /// Defines the contract for user management and authentication operations.
    /// </summary>
    /// <remarks>
    /// This interface provides operations for:
    /// <list type="bullet">
    /// <item><description>User authentication with email and password verification</description></item>
    /// <item><description>User retrieval by unique identifier</description></item>
    /// <item><description>User creation with secure password hashing using BCrypt</description></item>
    /// </list>
    /// </remarks>
    public interface IUserFunction
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>User object if found; null otherwise.</returns>
        Users? GetUserById(Guid userId);

        /// <summary>
        /// Authenticates a user by verifying email and password credentials.
        /// </summary>
        /// <remarks>
        /// The password is verified against the BCrypt-hashed password stored in the database.
        /// </remarks>
        /// <param name="email">User email address.</param>
        /// <param name="password">Plain text password to verify against stored hash.</param>
        /// <returns>User object if authentication is successful; null otherwise.</returns>
        Users? Authenticate(string email, string password);

        /// <summary>
        /// Creates a new user with a BCrypt hashed password.
        /// </summary>
        /// <remarks>
        /// The plain text password is automatically hashed using BCrypt before storage.
        /// BCrypt automatically generates and includes the salt in the hash, so no separate 
        /// salt storage is required.
        /// </remarks>
        /// <param name="user">User entity to create with email, full name, and role populated.</param>
        /// <param name="plainPassword">Plain text password to be hashed and stored.</param>
        void CreateUserWithHashedPassword(Users user, string plainPassword);
    }
}
