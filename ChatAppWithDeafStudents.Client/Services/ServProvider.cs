using ChatAppWithDeafStudents.Client.Services.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ChatAppWithDeafStudents.Client.Services
{
    public class ServProvider
    {
        private readonly HttpClient _httpClient;
        private const string TokenStorageKey = "auth_token";
        private const string RefreshTokenStorageKey = "refresh_token";
        private const string UserEmailStorageKey = "user_email";

        public ServProvider(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets the stored access token from secure storage.
        /// </summary>
        private async Task<string> GetStoredTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(TokenStorageKey);
                return token ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the stored access token synchronously (for SignalR).
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            return await GetStoredTokenAsync();
        }

        /// <summary>
        /// Stores the access token in secure storage.
        /// </summary>
        private async Task StoreTokenAsync(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await SecureStorage.SetAsync(TokenStorageKey, token);
                }
            }
            catch
            {
                // Logging could be added here if needed
            }
        }

        /// <summary>
        /// Stores the refresh token in secure storage.
        /// </summary>
        private async Task StoreRefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await SecureStorage.SetAsync(RefreshTokenStorageKey, refreshToken);
                }
            }
            catch
            {
                // Logging could be added here if needed
            }
        }

        /// <summary>
        /// Gets the stored refresh token from secure storage.
        /// </summary>
        public async Task<string> GetRefreshTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(RefreshTokenStorageKey);
                return token ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Stores the user email in secure storage.
        /// </summary>
        private async Task StoreUserEmailAsync(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    await SecureStorage.SetAsync(UserEmailStorageKey, email);
                }
            }
            catch
            {
                // Logging could be added here if needed
            }
        }

        /// <summary>
        /// Gets the stored user email from secure storage.
        /// </summary>
        public async Task<string> GetUserEmailAsync()
        {
            try
            {
                var email = await SecureStorage.GetAsync(UserEmailStorageKey);
                return email ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Аутенфикация запроса на вход.
        /// </summary>
        /// <param name="request">Запрос на вход.</param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, 
                    Encoding.UTF8, "application/json");

                var response = await _httpClient
                .PostAsync("/api/Authenticate/Auth", content);

                var jsonResponse = await response
                .Content.ReadAsStringAsync();

                var result = JsonConvert
                .DeserializeObject<AuthenticateResponse>(jsonResponse)
                    ?? new AuthenticateResponse();

                result.StatusCode = (int)response.StatusCode;

                if (result.StatusCode == 200 && !string.IsNullOrEmpty(result.Token))
                {
                    // Store access token in secure storage
                    await StoreTokenAsync(result.Token);

                    // Store refresh token in secure storage
                    if (!string.IsNullOrEmpty(result.RefreshTokenValue))
                    {
                        await StoreRefreshTokenAsync(result.RefreshTokenValue);
                    }

                    // Store user email in secure storage
                    if (!string.IsNullOrEmpty(result.Email))
                    {
                        await StoreUserEmailAsync(result.Email);
                    }
                }

                return result;

        }

        public async Task<TResponse> CallWebApi<TRequest, TResponse>(
            string apiUrl, HttpMethod httpMethod, TRequest? request) 
            where TResponse : BaseResponse, new()
        { 
            var message = new HttpRequestMessage(httpMethod, apiUrl);

            // Get token from secure storage
            var token = await GetStoredTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                message.Headers.Authorization 
                    = new AuthenticationHeaderValue("Bearer", token);
            }

            if (request != null)
            {
                string jsonContent 
                    = JsonConvert.SerializeObject(request);

                message.Content 
                    = new StringContent
                    (jsonContent, Encoding.UTF8, "application/json");
            }

            try
            {
                var response = await _httpClient.SendAsync(message);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new TResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        StatusMessage = $"Server returned " +
                        $"{response.StatusCode}: {jsonResponse}"
                    };
                }

                var result = JsonConvert.DeserializeObject
                    <TResponse>(jsonResponse);

                var finalResult = result ?? new TResponse();
                finalResult.StatusCode = (int)response.StatusCode;

                return finalResult;
            }

            catch (Exception ex)
            {
                var errorResult = new TResponse();
                errorResult.StatusCode = 500;
                errorResult.StatusMessage = ex.Message;
                return errorResult;
            }

        }
    }
}
