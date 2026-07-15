using ChatAppWithDeafStudents.Client.Pages; 

namespace ChatAppWithDeafStudents.Client
{
    public partial class AppShell : Shell
    {
        public AppShell(LoginPage loginPage)
        {
            InitializeComponent();

            Routing.RegisterRoute("ListChatPage", typeof(ListChatPage));

            this.CurrentItem = loginPage;
        }
    }
}
