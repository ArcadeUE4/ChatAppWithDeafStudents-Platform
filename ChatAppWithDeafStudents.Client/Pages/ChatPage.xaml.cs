using ChatAppWithDeafStudents.Client.ViewModel;

namespace ChatAppWithDeafStudents.Client.Pages;

public partial class ChatPage : ContentPage
{
	public ChatPage(ChatPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
	}
}