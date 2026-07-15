using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using ChatAppWithDeafStudents.Client.Configuration;

namespace ChatAppWithDeafStudents.Client.Services
{
    public class ChatHub
    {
        private readonly HubConnection _hubConnection;
        private readonly ServProvider _serviceProvider;
        private readonly ILogger<ChatHub> _logger;
        private readonly ApiSettings _apiSettings;

        public ChatHub(ServProvider serviceProvider, ILogger<ChatHub> logger, ApiSettings apiSettings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _apiSettings = apiSettings;

            // using URL from cofiguration
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_apiSettings.SignalRHub, options =>
                {
                    options.AccessTokenProvider = async () => await _serviceProvider.GetAccessTokenAsync();


                    #if DEBUG
                    options.HttpMessageHandlerFactory = (_) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    #endif
                })
                .WithAutomaticReconnect()
                .Build();

            
            _hubConnection.On<Guid, string>("ReceiveMessage", ReceiveMessageHandler);
        }

        // Private method to recive message
        private void ReceiveMessageHandler(Guid senderId, string content)
        {
            try
            {
                _logger.LogDebug($"Received message from {senderId}: {content}");
                // Invoke event
                OnReceiveMessage?.Invoke(senderId, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling received message");
            }
        }

        public async Task Connect()
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("SignalR Hub connected successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR Connect Error");
                throw;
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await _hubConnection.StopAsync();
                _logger.LogInformation("SignalR Hub disconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR Disconnect Error");
                throw;
            }
        }

        public async Task SendMessage(Guid senderId, Guid chatId, 
            string message, bool isVoiceToText)
        {
            try
            {
                await _hubConnection.InvokeAsync(
                    "SendMessage", chatId, message, 
                    isVoiceToText);

                _logger.LogInformation($"Message sent: " +
                    $"senderId={senderId}, chatId={chatId}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message");
                throw;
            }
        }

        public Action<Guid, string>? OnReceiveMessage { get; set; }
    }
}
