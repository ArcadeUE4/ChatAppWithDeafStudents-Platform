using ChatAppWithDeafStudents.API.Models;
using BCrypt.Net;

namespace ChatAppWithDeafStudents.API.Functions.User
{
    /// <summary>
    /// Implements user management and authentication 
    /// logic using BCrypt for secure password handling.
    /// </summary>
    public class UserFunction : IUserFunction
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserFunction> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFunction"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public UserFunction(ApplicationDbContext context, ILogger<UserFunction> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a user entity by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The <see cref="Users"/> object if found; otherwise, null.</returns>
        public Users? GetUserById(Guid userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        /// <summary>
        /// Authenticates a user by verifying email and password using BCrypt hashing.
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="password">Plain text password</param>
        /// <returns>User object if authentication is successful, null otherwise</returns>
        public Users? Authenticate(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning($"Login attempt with non-existent email: {email}");
                    return null;
                }

                // Verify password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning($"Failed login attempt for user: {email}");
                    return null;
                }

                _logger.LogInformation($"Successful login for user: {email}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Authentication error for email: {email}");
                return null;
            }
        }

        /// <summary>
        /// Creates a new user with a BCrypt hashed password.
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <param name="plainPassword">Plain text password to be hashed</param>
        public void CreateUserWithHashedPassword(Users user, string plainPassword)
        {
            // Hash the password using BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            _context.Users.Add(user);
            _context.SaveChanges();
            _logger.LogInformation($"User created: {user.Email}");
        }
    }
}
