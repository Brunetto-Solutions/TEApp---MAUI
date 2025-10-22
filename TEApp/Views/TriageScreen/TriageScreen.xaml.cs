namespace TEApp.Views.TriageScreen;

public partial class TriageScreen : ContentPage
{
	public TriageScreen()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QuestionsScreen.QuestionScreen());
    }
}