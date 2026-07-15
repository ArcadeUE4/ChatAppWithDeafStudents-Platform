using System.Collections.Concurrent;

namespace ChatAppWithDeafStudents.API.Services
{
    /// <summary>
    /// Manages refresh tokens for JWT-based authentication.
    /// </summary>
    /// <remarks>
    /// This service provides operations for generating, validating, and revoking refresh tokens.
    /// Refresh tokens are used to obtain new access tokens without requiring the user to re-authenticate.
    /// </remarks>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new refresh token for a user.
        /// </summary>
        /// <remarks>
        /// Creates a 256-bit cryptographically random token encoded as base64.
        /// The token is stored with metadata including expiration time and user ID.
        /// </remarks>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>A refresh token string (base64-encoded).</returns>
        /// <exception cref="InvalidOperationException">Thrown if token generation fails.</exception>
        string GenerateRefreshToken(Guid userId);

        /// <summary>
        /// Validates a refresh token to check if it is still valid and active.
        /// </summary>
        /// <remarks>
        /// A token is considered valid if:
        /// <list type="bullet">
        /// <item><description>It exists in the token cache</description></item>
        /// <item><description>It has not been revoked</description></item>
        /// <item><description>It has not expired</description></item>
        /// </list>
        /// </remarks>
        /// <param name="token">The refresh token to validate.</param>
        /// <returns>True if the token is valid and active; false otherwise.</returns>
        bool ValidateRefreshToken(string token);

        /// <summary>
        /// Retrieves the user ID associated with a valid refresh token.
        /// </summary>
        /// <remarks>
        /// This method validates the token before extracting the user ID.
        /// If the token is invalid, null is returned.
        /// </remarks>
        /// <param name="token">The refresh token.</param>
        /// <returns>The user ID if the token is valid; null otherwise.</returns>
        Guid? GetUserIdFromRefreshToken(string token);

        /// <summary>
        /// Revokes a single refresh token, preventing its further use.
        /// </summary>
        /// <remarks>
        /// Revoked tokens remain in the cache until manually cleaned up by <see cref="CleanupExpiredTokens"/>.
        /// </remarks>
        /// <param name="token">The refresh token to revoke.</param>
        void RevokeRefreshToken(string token);

        /// <summary>
        /// Revokes all refresh tokens for a user, effectively logging them out from all sessions.
        /// </summary>
        /// <remarks>
        /// This operation is typically called during user logout or password change.
        /// All active tokens for the user become invalid immediately.
        /// </remarks>
        /// <param name="userId">The user's unique identifier.</param>
        void RevokeAllUserTokens(Guid userId);

        /// <summary>
        /// Cleans up expired tokens from the in-memory cache to prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// This method should be called periodically (e.g., via a background service or scheduled task)
        /// to remove tokens that have passed their expiration time.
        /// </remarks>
        void CleanupExpiredTokens();
    }

    /// <summary>
    /// Implementation of <see cref="ITokenService"/> using in-memory cache with ConcurrentDictionary.
    /// </summary>
    /// <remarks>
    /// This implementation stores refresh tokens in memory with expiration tracking.
    /// It is suitable for single-instance deployments but not recommended for distributed systems.
    /// For production distributed systems, consider using Redis or another persistent cache.
    /// </remarks>
    public class TokenService : ITokenService
    {
        private readonly ConcurrentDictionary<string, TokenMetadata> _refreshTokens = new();
        private readonly ILogger<TokenService> _logger;
        private readonly int _refreshTokenExpirationMinutes;

        private class TokenMetadata
        {
            public Guid UserId { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool IsRevoked { get; set; }
            public DateTime CreatedAt { get; set; }
            public string IssuedFromIp { get; set; } = string.Empty;

            public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <remarks>
        /// The refresh token expiration time is configured via the "Jwt:RefreshTokenExpirationMinutes" 
        /// setting in the application configuration. If not configured, defaults to 10080 minutes (7 days).
        /// </remarks>
        /// <param name="configuration">The application configuration containing JWT settings.</param>
        /// <param name="logger">The logger instance for logging token operations.</param>
        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _refreshTokenExpirationMinutes = configuration.GetValue<int>("Jwt:RefreshTokenExpirationMinutes", 10080); // Default: 7 days
        }

        /// <summary>
        /// Generates a new refresh token (256-bit random token encoded as base64).
        /// </summary>
        /// <remarks>
        /// Uses <see cref="System.Security.Cryptography.RandomNumberGenerator"/> to generate 
        /// cryptographically secure random bytes, then encodes them as base64 for transport and storage.
        /// </remarks>
        /// <param name="userId">The user's unique identifier to associate with this token.</param>
        /// <returns>A refresh token string (base64-encoded).</returns>
        /// <exception cref="InvalidOperationException">Thrown if the token cannot be added to the cache (rare case).</exception>
        public string GenerateRefreshToken(Guid userId)
        {
            // Generate 256-bit (32-byte) random token
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var token = Convert.ToBase64String(randomNumber);
            var metadata = new TokenMetadata
            {
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_refreshTokenExpirationMinutes),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                IssuedFromIp = string.Empty
            };

            if (_refreshTokens.TryAdd(token, metadata))
            {
                _logger.LogInformation($"Generated refresh token for user: {userId}");
                return token;
            }

            throw new InvalidOperationException("Failed to generate refresh token");
        }

        /// <summary>
        /// Validates if a refresh token is still valid (not expired, not revoked, and exists in the cache).
        /// </summary>
        /// <remarks>
        /// Logs diagnostic information at different levels:
        /// <list type="bullet">
        /// <item><description>WARNING level if token not found or validation fails</description></item>
        /// <item><description>DEBUG level would be used for successful validations</description></item>
        /// </list>
        /// </remarks>
        /// <param name="token">The refresh token to validate.</param>
        /// <returns>True if the token is valid and can be used; false otherwise.</returns>
        public bool ValidateRefreshToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            if (!_refreshTokens.TryGetValue(token, out var metadata))
            {
                _logger.LogWarning($"Refresh token not found: {token.Substring(0, Math.Min(10, token.Length))}...");
                return false;
            }

            if (!metadata.IsValid)
            {
                _logger.LogWarning($"Refresh token is invalid for user {metadata.UserId} (revoked: {metadata.IsRevoked}, expired: {metadata.ExpiresAt <= DateTime.UtcNow})");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the user ID associated with a valid refresh token.
        /// </summary>
        /// <remarks>
        /// This method validates the token before extracting the user ID.
        /// If the token is invalid, null is returned.
        /// </remarks>
        /// <param name="token">The refresh token.</param>
        /// <returns>The user ID if the token is valid; null otherwise.</returns>
        public Guid? GetUserIdFromRefreshToken(string token)
        {
            if (!ValidateRefreshToken(token))
                return null;

            if (_refreshTokens.TryGetValue(token, out var metadata))
                return metadata.UserId;

            return null;
        }

        /// <summary>
        /// Revokes a single refresh token, preventing its further use.
        /// </summary>
        /// <remarks>
        /// Revoked tokens remain in the cache until manually cleaned up by <see cref="CleanupExpiredTokens"/>.
        /// </remarks>
        /// <param name="token">The refresh token to revoke.</param>
        public void RevokeRefreshToken(string token)
        {
            if (_refreshTokens.TryGetValue(token, out var metadata))
            {
                metadata.IsRevoked = true;
                _logger.LogInformation($"Revoked refresh token for user: {metadata.UserId}");
            }
        }

        /// <summary>
        /// Revokes all refresh tokens for a user, effectively logging them out from all sessions.
        /// </summary>
        /// <remarks>
        /// This operation is typically called during user logout or password change.
        /// All active tokens for the user become invalid immediately.
        /// </remarks>
        /// <param name="userId">The user's unique identifier.</param>
        public void RevokeAllUserTokens(Guid userId)
        {
            var userTokens = _refreshTokens
                .Where(kvp => kvp.Value.UserId == userId && !kvp.Value.IsRevoked)
                .ToList();

            foreach (var kvp in userTokens)
            {
                kvp.Value.IsRevoked = true;
            }

            _logger.LogInformation($"Revoked all refresh tokens for user: {userId}. Count: {userTokens.Count}");
        }

        /// <summary>
        /// Cleans up expired tokens from the in-memory cache to prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// This method should be called periodically (e.g., via a background service or scheduled task)
        /// to remove tokens that have passed their expiration time.
        /// </remarks>
        public void CleanupExpiredTokens()
        {
            var expiredTokens = _refreshTokens
                .Where(kvp => kvp.Value.ExpiresAt <= DateTime.UtcNow)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                if (_refreshTokens.TryRemove(token, out _))
                {
                    _logger.LogDebug($"Removed expired refresh token");
                }
            }

            if (expiredTokens.Count > 0)
                _logger.LogInformation($"Cleaned up {expiredTokens.Count} expired refresh tokens. Cache size: {_refreshTokens.Count}");
        }
    }
}
