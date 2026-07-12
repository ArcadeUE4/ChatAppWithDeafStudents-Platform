using ChatAppWithDeafStudents.Client.Converters;
using ChatAppWithDeafStudents.Client.Helpers;
using ChatAppWithDeafStudents.Client.Models;
using ChatAppWithDeafStudents.Client.Services;
using ChatAppWithDeafStudents.Client.Services.Message;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ChatAppWithDeafStudents.Client.ViewModel
{
    public partial class ChatPageViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ServProvider _serviceProvider;
        private readonly ChatHub _chatHub;
        private readonly ISpeechToText _speechToText;
        private readonly ILogger<ChatPageViewModel> _logger;

        [ObservableProperty]
        private Guid chatId = Guid.Empty;

        [ObservableProperty]
        private Guid senderId = Guid.Empty;

        [ObservableProperty]
        private ObservableCollection<Messages> messages = new();

        [ObservableProperty]
        private string content = string.Empty;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string chatTitle = string.Empty;

        [ObservableProperty]
        private Users friendInfo = new();

        public ChatPageViewModel(ServProvider serviceProvider, 
            ChatHub chatHub, 
            ISpeechToText speechToText, 
            ILogger<ChatPageViewModel> logger)
        {
            _serviceProvider = serviceProvider;
            _chatHub = chatHub;
            _speechToText = speechToText;
            _logger = logger;
            _chatHub.OnReceiveMessage += OnReceiveMessage;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                _logger.LogDebug($"ApplyQueryAttributes called with " +
                    $"{query.Count} parameters");
                foreach (var kvp in query)
                {
                    _logger.LogDebug($"  Param: {kvp.Key} = {kvp.Value}");
                }

                if (query.TryGetValue("chatId", out var cId))
                {
                    _logger.LogDebug($"Found chatId: {cId}");
                    if (Guid.TryParse(cId.ToString(), out var chatId))
                    {
                        ChatId = chatId;
                        _logger.LogDebug($"ChatId set to: {ChatId}");
                    }
                }

                if (query.TryGetValue("userId", out var uId))
                {
                    _logger.LogDebug($"Found userId: {uId}");
                    if (Guid.TryParse(uId.ToString(), out var userId))
                    {
                        SenderId = userId;
                        MessagePositionConverter.CurrentUserId = userId;
                        MessageColorConverter.CurrentUserId = userId;
                        SenderNameVisibilityConverter.CurrentUserId = userId;
                        _logger.LogDebug($"SenderId set to: {SenderId}");
                    }
                }

                
                _logger.LogDebug("Calling Initialize from ApplyQueryAttributes");
                Initialize();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyQueryAttributes error");
                Shell.Current.DisplayAlertAsync("Error", $"Error in loading messages: {ex.Message}", "OK");
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Content)) 
                return;

            try
            {
                await _chatHub.SendMessage(SenderId, ChatId, Content, false);

                Messages.Add(new Messages
                {
                    Content = Content,
                    SenderId = SenderId,
                    IsVoiceToText = false,
                    CreatedAt = DateTime.UtcNow
                });

                Content = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task Listen()
        {
            var isAuthorized = await _speechToText.RequestPermissions();
            if (!isAuthorized)
            {
                await Shell.Current.DisplayAlertAsync("Error", "No permisson on microphone", "OK");
                return;
            }

            try
            {
                var result = await _speechToText.Listen(
                    CultureInfo.GetCultureInfo("ru-ru"),
                    new Progress<string>(),
                    CancellationToken.None);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    Content = result;
                    await _chatHub.SendMessage(SenderId, ChatId, Content, true);

                    Messages.Add(new Messages
                    {
                        Content = Content,
                        SenderId = SenderId,
                        IsVoiceToText = true,
                        CreatedAt = DateTime.UtcNow
                    });

                    Content = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private async Task GetMessages()
        {
            try
            {
                var request = new MessageInitializeRequest
                {
                    ChatId = ChatId,
                    UserId = SenderId,
                };

                _logger.LogInformation($"GetMessages: requesting chat {ChatId} for user {SenderId}");

                var response = await _serviceProvider.CallWebApi<
                    MessageInitializeRequest, 
                    MessageInitializeResponse>
                    ("api/Message/Initialize", HttpMethod.Post, request);

                _logger.LogInformation($"GetMessages: response received - StatusCode: {response?.StatusCode}, MessageCount: {response?.Messages?.Count() ?? 0}");

                if (response != null && response.StatusCode == 200)
                {
                    _logger.LogInformation($"GetMessages: loading {response.Messages.Count()} messages");
                    Messages = new ObservableCollection<Messages>(response.Messages);


                    if (response.ChatInfo != null && !string.IsNullOrEmpty(response.ChatInfo.Title))
                    {
                        ChatTitle = response.ChatInfo.Title;
                        _logger.LogInformation($"GetMessages: chat title set to '{ChatTitle}'");
                    }


                    if (response.FriendInfo != null)
                    {
                        FriendInfo = response.FriendInfo;
                        _logger.LogInformation($"GetMessages: friend info set to '{FriendInfo.FullName}'");
                    }
                }
                else
                {
                    _logger.LogWarning($"GetMessages: failed with status {response?.StatusCode}, message: {response?.StatusMessage}");
                    await Shell.Current.DisplayAlertAsync("Ошибка", 
                        response?.StatusMessage ?? "Error message loading", "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMessages: exception occurred");
                await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "OK");
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task Refresh()
        {
            if (IsRefreshing) 
                return;

            IsRefreshing = true;
            try
            {
                await GetMessages();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void OnReceiveMessage(Guid senderId, string content)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new Messages
                {
                    Content = content,
                    SenderId = senderId,
                    CreatedAt = DateTime.UtcNow
                });
            });
        }

        public async void Initialize()
        {
            try
            {
                _logger.LogInformation($"Initialize: " +
                    $"ChatId={ChatId}, " +
                    $"SenderId={SenderId}");

                
                if (_chatHub != null)
                {
                    try
                    {
                        await _chatHub.Connect();
                        _logger.LogInformation("ChatHub connected successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ChatHub Connect error");
                        await Shell.Current.DisplayAlertAsync("Error connection", $"SignalR: {ex.Message}", "OK");
                        return;
                    }

                    try
                    {
                        await _chatHub.JoinChat(ChatId);
                        _logger.LogInformation($"Joined chat: {ChatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "JoinChat error");
                        await Shell.Current.DisplayAlertAsync("Error connection", $"JoinChat: {ex.Message}", "OK");
                        return;
                    }
                }

                await GetMessages();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Initialize error");
                await Shell.Current.DisplayAlertAsync("Error", 
                    $"{ex.Message}", "OK");
            }
        }
    }
}



