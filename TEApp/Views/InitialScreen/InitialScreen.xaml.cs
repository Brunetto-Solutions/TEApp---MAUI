using TEApp.Views.TherapeuticScreen;

namespace TEApp.Views.InitialScreen;

public partial class InitialScreen : ContentPage
{
	public InitialScreen()
	{
		InitializeComponent();
	}

    private async void OnTriageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TriageScreen.TriageScreen());
    }

    private async void onDiagnosticClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TherapeuticScreen.TherapeuticScreen());
    }
}