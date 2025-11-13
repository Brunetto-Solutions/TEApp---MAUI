namespace TEApp.Views.TherapeuticScreen;

public partial class TherapeuticScreen : ContentPage
{
	public TherapeuticScreen()
	{
		InitializeComponent();
	}

    private async void GoHome(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private async void PlanTherapeutic(object sender, EventArgs e)
    {
        await DisplayAlert("Link", "Função de compartilhamento em desenvolvimento", "OK");
    }
}