using System.Collections.Generic;
using System.Linq;

namespace TEApp.Views.QuestionsScreen;

public partial class QuestionScreen : ContentPage
{
    private List<Question> allQuestions;
    private List<Question> selectedQuestions;
    private Dictionary<string, string> userAnswers = new Dictionary<string, string>();

    public QuestionScreen()
    {
        InitializeComponent();
        LoadQuestions();
    }

    private void LoadQuestions()
    {
        // Cria as perguntas diretamente no código
        allQuestions = new List<Question>
        {
            new Question
            {
                Id = "q1",
                Text = "Prefiro fazer as coisas com os outros do que sozinho.",
                Type = "radio",
                Options = new Dictionary<string, string>
                {
                    { "1", "Definitivamente concordo" },
                    { "2", "Concordo um pouco" },
                    { "3", "Discordo um pouco" },
                    { "4", "Definitivamente discordo" }
                }
            },
            new Question
            {
                Id = "q2",
                Text = "Prefiro fazer as coisas sempre da mesma maneira",
                Type = "radio",
                Options = new Dictionary<string, string>
                {
                    { "1", "Concordo plenamente" },
                    { "2", "Concordo um pouco" },
                    { "3", "Discordo um pouco" },
                    { "4", "Discordo plenamente" }
                }
            },
            new Question
            {
                Id = "q3",
                Text = "Quando tento imaginar uma coisa, tenho muita facilidade em criar uma imagem na minha mente.",
                Type = "radio",
                Options = new Dictionary<string, string>
                {
                    { "1", "Concordo plenamente" },
                    { "2", "Concordo um pouco" },
                    { "3", "Discordo um pouco" },
                    { "4", "Discordo plenamente" }
                }
            },
            new Question
            {
                Id = "q4",
                Text = "Frequentemente fico tão absorto em uma coisa que ignoro outras coisas.",
                Type = "radio",
                Options = new Dictionary<string, string>
                {
                    { "1", "Definitivamente concordo" },
                    { "2", "Concordo um pouco" },
                    { "3", "Discordo um pouco" },
                    { "4", "Definitivamente discordo" }
                }
            },
            new Question
            {
                Id = "q5",
                Text = "Frequentemente noto pequenos sons quando outros não notam.",
                Type = "radio",
                Options = new Dictionary<string, string>
                {
                    { "1", "Definitivamente concordo" },
                    { "2", "Concordo um pouco" },
                    { "3", "Discordo um pouco" },
                    { "4", "Definitivamente discordo" }
                }
            }
        };

        // Embaralha e seleciona 3 perguntas aleatórias
        selectedQuestions = allQuestions
            .OrderBy(x => Guid.NewGuid())
            .Take(5)
            .ToList();

        DisplayQuestions();
    }

    private void DisplayQuestions()
    {
        QuestionsStackLayout.Children.Clear();
        int questionIndex = 1;

        foreach (var question in selectedQuestions)
        {
            // Frame para cada pergunta (estilo verde/azulado)
            var questionFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#2C5F6F"),
                CornerRadius = 15,
                Padding = 15,
                Margin = new Thickness(0, 10),
                HasShadow = true
            };

            var questionLayout = new VerticalStackLayout { Spacing = 8 };

            // Título da pergunta
            questionLayout.Children.Add(new Label
            {
                Text = $"{questionIndex}. {question.Text}",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Opções de resposta
            if (question.Options != null && question.Options.Count > 0)
            {
                string groupName = $"question_{question.Id}";

                foreach (var optionPair in question.Options.OrderBy(o => o.Key))
                {
                    var optionLayout = new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Margin = new Thickness(5, 3)
                    };

                    var radioButton = new RadioButton
                    {
                        Value = optionPair.Key,
                        GroupName = groupName,
                        BackgroundColor = Colors.Transparent
                    };

                    // Captura o ID da questão para salvar resposta
                    string currentQuestionId = question.Id;
                    radioButton.CheckedChanged += (s, e) =>
                    {
                        if (e.Value)
                        {
                            userAnswers[currentQuestionId] = optionPair.Key;
                        }
                    };

                    var optionLabel = new Label
                    {
                        Text = optionPair.Value,
                        TextColor = Colors.White,
                        FontSize = 13,
                        VerticalOptions = LayoutOptions.Center
                    };

                    optionLayout.Children.Add(radioButton);
                    optionLayout.Children.Add(optionLabel);

                    questionLayout.Children.Add(optionLayout);
                }
            }

            questionFrame.Content = questionLayout;
            QuestionsStackLayout.Children.Add(questionFrame);

            questionIndex++;
        }
    }

    private async void OnFinishClicked(object sender, EventArgs e)
    {
        // Verifica se todas as perguntas foram respondidas
        if (userAnswers.Count < selectedQuestions.Count)
        {
            await DisplayAlert("Atenção",
                "Por favor, responda todas as perguntas antes de finalizar.",
                "OK");
            return;
        }

        // Calcula resultado
        string resultado = CalcularResultado();

        await DisplayAlert("Questionário Finalizado!", resultado, "OK");

        // Navega para a HomePage
        await Navigation.PushAsync(new HomePage());
    }
    private string CalcularResultado()
    {
        int pontuacao = 0;

        foreach (var resposta in userAnswers)
        {
            // Converte a opção escolhida para pontuação
            if (int.TryParse(resposta.Value, out int opcao))
            {
                pontuacao += opcao;
            }
        }

        return $"Suas respostas:\n" +
               $"Total de perguntas respondidas: {userAnswers.Count}\n" +
               $"Pontuação total: {pontuacao}\n\n" +
               string.Join("\n", userAnswers.Select(kvp =>
                   $"{kvp.Key}: Opção {kvp.Value}"));
    }
}

public class Question
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Options { get; set; }
}