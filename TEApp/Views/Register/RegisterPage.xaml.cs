using TEApp.Views.Login;
namespace TEApp.Views.Register;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private async void loginRedirect(object sender, EventArgs e)
    {
        var loginPage = this.Handler.MauiContext.Services.GetService<LoginPage>();
        await Navigation.PushAsync(loginPage);
    }
}