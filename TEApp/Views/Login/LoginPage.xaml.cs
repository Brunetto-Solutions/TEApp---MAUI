using Microsoft.Maui.Controls;

namespace TEApp.Views.Login
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void loginRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InitialScreen.InitialScreen());
        }

        private async void registerRedirect(object sender, EventArgs e)
        {

        }
    }
}
