using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace TEApp.Views.QuestionsScreen;

public partial class QuestionScreen : ContentPage
{
    private FirebaseClient firebaseClient;
    private List<Question> allQuestions;
    private List<Question> selectedQuestions;

    public QuestionScreen()
    {
        InitializeComponent();
        firebaseClient = new FirebaseClient("https://teapp-120925-default-rtdb.firebaseio.com/");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadQuestions();
    }

    private async void LoadQuestions()
    {
        try
        {
            var questionsDict = await firebaseClient
                .Child("forms")
                .Child("aq_test")
                .Child("questions")
                .OnceAsync<Dictionary<string, Question>>();

            allQuestions = questionsDict
                .SelectMany(d => d.Object.Values)
                .ToList();

            if (allQuestions.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhuma pergunta encontrada.", "OK");
                return;
            }

            // Seleciona 3 aleatórias
            selectedQuestions = allQuestions
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();

            DisplayQuestions();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Falha ao carregar perguntas: " + ex.Message, "OK");
        }
    }

    private void DisplayQuestions()
    {
        QuestionsStackLayout.Children.Clear();
        int index = 1;

        foreach (var question in selectedQuestions)
        {
            QuestionsStackLayout.Children.Add(new Label
            {
                Text = $"{index}. {question.Text}",
                FontSize = 16,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 10)
            });

            // Converte Dictionary para lista de opções
            var optionsList = question.Options.Values.ToList();

            foreach (var option in optionsList)
            {
                var horizontalLayout = new HorizontalStackLayout { Spacing = 10 };

                var radioButton = new RadioButton
                {
                    Value = option,
                    GroupName = $"question{index}"
                };

                horizontalLayout.Children.Add(radioButton);
                horizontalLayout.Children.Add(new Label { Text = option, VerticalOptions = LayoutOptions.Center });

                QuestionsStackLayout.Children.Add(horizontalLayout);
            }

            index++;
        }
    }

    private void OnFinishClicked(object sender, EventArgs e)
    {
        DisplayAlert("Questionário", "Você finalizou o questionário!", "OK");
    }
}

public class Question
{
    [JsonProperty("question")]
    public string Text { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("options")]
    public Dictionary<string, string> Options { get; set; }
}
