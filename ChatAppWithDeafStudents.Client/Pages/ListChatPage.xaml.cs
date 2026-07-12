using ChatAppWithDeafStudents.Client.ViewModel;

namespace ChatAppWithDeafStudents.Client.Pages;

public partial class ListChatPage : ContentPage
{
	public ListChatPage(ListChatPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}