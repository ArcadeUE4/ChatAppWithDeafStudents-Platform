using ChatAppWithDeafStudents.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatAppWithDeafStudents.Client.Services.Authentication;

namespace ChatAppWithDeafStudents.Client.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly ServProvider _serviceProvider;

        [ObservableProperty]
        private string _email = "john@example.com";

        [ObservableProperty]
        private string _password = "password123";

        [ObservableProperty]
        private bool _isProcessing;

        public LoginPageViewModel(ServProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", 
                    "Write all fields.", "OK");
                return;
            }

            IsProcessing = true;
            try
            {
                var request = new AuthenticateRequest
                {
                    email = Email,
                    password = Password,
                };

                var response = await _serviceProvider.Authenticate(request);

                if (response.StatusCode == 200)
                {
                    await Shell.Current.GoToAsync($"ListChatPage?Id={response.Id}");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", 
                        response?.StatusMessage ?? "Authennication error", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    ex.Message, "OK");
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
