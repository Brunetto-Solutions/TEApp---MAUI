namespace TEApp.Views.Register;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private async void loginRedirect(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Login.LoginPage());
    }
}