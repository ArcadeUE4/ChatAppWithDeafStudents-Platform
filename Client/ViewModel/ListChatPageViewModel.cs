using ChatAppWithDeafStudents.Client.Models;
using ChatAppWithDeafStudents.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ChatAppWithDeafStudents.Client.ViewModel
{
    public partial class ListChatPageViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ServProvider _serviceProvider;
        private readonly ChatHub _chathub;
        
        [ObservableProperty]
        private ObservableCollection<ChatMembers> 
            userChats = new();

        [ObservableProperty]
        private ObservableCollection<LastestMessages> 
            lastestMessages = new();

        [ObservableProperty] 
        private Users _userInfo = new();
        
        [ObservableProperty] 
        private bool _isRefreshing;

        public ListChatPageViewModel(
            ServProvider serviceProvider, 
            ChatHub chatHub)
        {

            _serviceProvider = serviceProvider;
            _chathub = chatHub;
            _chathub.OnReceiveMessage += OnReceivedMessage;

        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task Refresh()
        {
            if (IsRefreshing) return;
            IsRefreshing = true;
            try
            {
            await GetListChats();
            }
            finally
            {
            IsRefreshing = false;
        }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task OpenPage(Guid chatId)
        {
            await Shell.Current.GoToAsync($"///ChatPage?userId=" +
                $"{UserInfo.Id}&chatId={chatId}");
        }

        private async Task GetListChats()
        {
            try
            {
                var response = await _serviceProvider.
                    CallWebApi<Guid, ListChatResponse>
                    ("api/Chat/Initialize", HttpMethod.Post, 
                    UserInfo.Id);

                if (response.StatusCode == 200)
                {
                    if (response.Users != null)
                    {
                        UserInfo = response.Users;
                    }

                    UserChats.Clear();


                    foreach (var chat in response.ChatMembers ?? Enumerable.Empty<ChatMembers>())
                    {

                        var lastMessage = response.LastestMessages?
                            .FirstOrDefault(m => m.ChatId == chat.ChatId);

                        
                        if (lastMessage != null)
                        {
                            chat.LastestMessage = lastMessage;
                        }
                        else
                        {
                            
                            chat.LastestMessage = new LastestMessages
                            {
                                Content = "",
                                SendDateTime = DateTime.MinValue,
                                ChatId = chat.ChatId
                            };
                        }

                        UserChats.Add(chat);
                    }

                    LastestMessages.Clear();
                    if (response.LastestMessages != null)
                    {
                        foreach (var msg in response.LastestMessages) 
                        {
                            LastestMessages.Add(msg);
                        }
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", 
                        response.StatusMessage ?? 
                        "Ошибка загрузки чатов", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    ex.Message, "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary
            <string, object> query)
        {
            try
            {
                if (query.TryGetValue("Id", out var cId))
                {
                    if (Guid.TryParse(cId.ToString(), 
                        out var userId))
                    {
                        UserInfo.Id = userId;
                        _ = GetListChats();
                    }

                    else
                    {
                        Shell.Current.DisplayAlertAsync
                            ("Error", "Wrong userID", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlertAsync(
                    "Error", 
                    $"Error loading parametrs: " +
                    $"{ex.Message}", "OK");
            }
        }

        private void OnReceivedMessage(Guid senderId, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                {
                    var existingMessage = LastestMessages.
                    FirstOrDefault(x => x.UserId == senderId);

                    if (existingMessage != null)
                    { 
                        LastestMessages.Remove(existingMessage);
                    }
                    
                    var newLastestMessage = new LastestMessages
                    {
                        Id = Guid.NewGuid(),
                    UserId = senderId,
                        Content = message,
                        SendDateTime = DateTime.Now,
                        IsRead = false,
                    };

                    LastestMessages.Insert(0, newLastestMessage);
                    OnPropertyChanged(nameof(LastestMessages));
                });
        }
    }
}
