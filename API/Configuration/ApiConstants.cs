namespace ChatAppWithDeafStudents.API.Configuration
{
    /// <summary>
    /// Contains constants for API configuration, endpoints, and magic numbers.
    /// </summary>
    public static class ApiConstants
    {
        /// <summary>
        /// SignalR Hub Routes
        /// </summary>
        public static class SignalR
        {
            /// <summary>
            /// The route path for the SignalR chat hub.
            /// </summary>
            public const string HubRoute = "/chathub";
        }

        /// <summary>
        /// API Route Prefixes
        /// </summary>
        public static class Routes
        {
            /// <summary>
            /// The API version prefix used in all API endpoints.
            /// </summary>
            public const string ApiPrefix = "api";

            /// <summary>
            /// The route template for authentication endpoints.
            /// </summary>
            public const string AuthenticateRoute = "api/[controller]";

            /// <summary>
            /// The route template for chat endpoints.
            /// </summary>
            public const string ChatRoute = "api/[controller]";

            /// <summary>
            /// The route template for message endpoints.
            /// </summary>
            public const string MessageRoute = "api/[controller]";
        }

        /// <summary>
        /// CORS Policy Names
        /// </summary>
        public static class CorsPolicy
        {
            /// <summary>
            /// The name of the specific CORS policy that allows requests from configured origins.
            /// </summary>
            public const string AllowSpecific = "AllowSpecific";
        }

        /// <summary>
        /// Authentication Configuration
        /// </summary>
        public static class Authentication
        {
            /// <summary>
            /// The expiration time for JWT tokens in hours.
            /// </summary>
            public const int JwtTokenExpirationHours = 24;

            /// <summary>
            /// The authentication scheme name for JWT Bearer tokens.
            /// </summary>
            public const string JwtBearerScheme = "Bearer";
        }

        /// <summary>
        /// Password Security Configuration
        /// </summary>
        public static class PasswordSecurity
        {
            /// <summary>
            /// The size of the salt used in password hashing (in bytes).
            /// </summary>
            public const int SaltSize = 16;

            /// <summary>
            /// The size of the derived key for password hashing (in bytes).
            /// </summary>
            public const int KeySize = 32;

            /// <summary>
            /// The number of iterations for password hashing algorithms like PBKDF2.
            /// </summary>
            public const int Iterations = 10000;
        }

        /// <summary>
        /// Logging Configuration
        /// </summary>
        public static class Logging
        {
            /// <summary>
            /// The default log level for application logging.
            /// </summary>
            public const string DefaultLogLevel = "Information";

            /// <summary>
            /// The log level for ASP.NET Core framework logging.
            /// </summary>
            public const string AspNetCoreLogLevel = "Warning";
        }

        /// <summary>
        /// Error Messages
        /// </summary>
        public static class ErrorMessages
        {
            /// <summary>
            /// Error message when the JWT key is not found in the configuration.
            /// </summary>
            public const string JwtKeyNotFound = "JWT key not found in configuration.";

            /// <summary>
            /// Error message for invalid email or password credentials.
            /// </summary>
            public const string InvalidEmailOrPassword = "Invalid email or password";

            /// <summary>
            /// Error message when email or password fields are missing.
            /// </summary>
            public const string EmailAndPasswordRequired = "Email and password are required";

            /// <summary>
            /// Error message for general authentication failures.
            /// </summary>
            public const string AuthenticationError = "An error occurred during authentication";

            /// <summary>
            /// Error message for database-related errors.
            /// </summary>
            public const string DatabaseError = "Database error";

            /// <summary>
            /// Generic error message for unexpected errors.
            /// </summary>
            public const string UnexpectedError = "An error occurred";

            /// <summary>
            /// Error message when the user is not authenticated or has an invalid user ID.
            /// </summary>
            public const string UserNotAuthenticated = "User is not authenticated or has invalid user ID";
        }

        /// <summary>
        /// Success Messages
        /// </summary>
        public static class SuccessMessages
        {
            /// <summary>
            /// Success message for successful authentication.
            /// </summary>
            public const string AuthenticationSuccess = "Authentication successful";

            /// <summary>
            /// Success message for successful initialization.
            /// </summary>
            public const string InitializationSuccess = "OK";

            /// <summary>
            /// Success message for successful chat initialization.
            /// </summary>
            public const string ChatInitializedSuccess = "Success";
        }

        /// <summary>
        /// HTTP Status Codes
        /// </summary>
        public static class StatusCodes
        {
            /// <summary>
            /// HTTP 200 - Successful request.
            /// </summary>
            public const int Success = 200;

            /// <summary>
            /// HTTP 400 - Bad request due to invalid data.
            /// </summary>
            public const int BadRequest = 400;

            /// <summary>
            /// HTTP 401 - Unauthorized; authentication is required.
            /// </summary>
            public const int Unauthorized = 401;

            /// <summary>
            /// HTTP 403 - Forbidden; the user does not have permission.
            /// </summary>
            public const int Forbidden = 403;

            /// <summary>
            /// HTTP 404 - Not found; the requested resource does not exist.
            /// </summary>
            public const int NotFound = 404;

            /// <summary>
            /// HTTP 500 - Internal server error.
            /// </summary>
            public const int ServerError = 500;
        }

        /// <summary>
        /// Pagination Configuration
        /// </summary>
        public static class Pagination
        {
            /// <summary>
            /// The default number of items per page for paginated API responses.
            /// </summary>
            public const int DefaultPageSize = 50;

            /// <summary>
            /// The maximum number of items allowed per page to prevent excessively large responses.
            /// </summary>
            public const int MaxPageSize = 100;
        }
    }
}
