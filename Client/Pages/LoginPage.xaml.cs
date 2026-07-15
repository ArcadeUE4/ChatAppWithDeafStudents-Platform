using ChatAppWithDeafStudents.Client.ViewModel;

namespace ChatAppWithDeafStudents.Client.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}