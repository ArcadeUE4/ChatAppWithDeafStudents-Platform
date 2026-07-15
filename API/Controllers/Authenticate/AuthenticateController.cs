using ChatAppWithDeafStudents.API.Functions.User;
using ChatAppWithDeafStudents.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatAppWithDeafStudents.API.Controllers.Authenticate
{
    /// <summary>
    /// Handles user authentication requests 
    /// and token generation.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserFunction _userFunction;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateController"/> 
        /// class.
        /// </summary>
        /// <param name="userFunction">The user authentication service.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenService">The token service for managing refresh tokens.</param>
        public AuthenticateController(IUserFunction userFunction, IConfiguration configuration, ILogger<AuthenticateController> logger, ITokenService tokenService)
        {
            _userFunction = userFunction;
            _configuration = configuration;
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user with email and password credentials.
        /// </summary>
        /// <param name="request">The authentication request containing email and password.</param>
        /// <returns>
        /// <see cref="OkResult"/> with <see cref="AuthenticateTokenResponse"/> containing the access token and refresh token if authentication is successful.
        /// <see cref="UnauthorizedResult"/> if the credentials are invalid.
        /// <see cref="BadRequestResult"/> if the request is invalid.
        /// </returns>
        /// <response code="200">Authentication successful - returns tokens</response>
        /// <response code="400">Invalid request data or model state</response>
        /// <response code="401">Invalid email or password</response>
        /// <response code="500">Internal server error during authentication</response>
        [HttpPost("Auth")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            try
            {
                // Validate ModelState using DataAnnotations
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Authentication attempt with invalid model state");
                    return BadRequest(ModelState);
                }

                // Validate input data
                if (request == null || string.IsNullOrWhiteSpace(request.email) ||
                    string.IsNullOrWhiteSpace(request.password))
                {
                    _logger.LogWarning("Authentication attempt with invalid request data");
                    return BadRequest(new { message = "Email and password are required" });
                }

                var user = _userFunction.Authenticate(request.email, request.password);

                if (user == null)
                {
                    _logger.LogWarning($"Failed authentication attempt for email: {request.email}");
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Generate JWT access token
                var accessToken = GenerateAccessToken(user.Id, user.Email);

                // Generate refresh token
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

                _logger.LogInformation($"Successful authentication for user: {user.Email}");

                // Set refresh token in HttpOnly cookie for security
                Response.Cookies.Append("refreshToken", refreshToken, 
                    new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationMinutes", 10080))
                });

                return Ok(new AuthenticateTokenResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = 900,
                    StatusCode = 200,
                    StatusMessage = "Authentication successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication error");
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }

        /// <summary>
        /// Generates a short-lived JWT access token.
        /// </summary>
        /// <param name="userId">The user's unique identifier</param>
        /// <param name="email">The user's email address</param>
        /// <returns>A JWT token string</returns>
        private string GenerateAccessToken(Guid userId, string email)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("token_type", "access_token"),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

