using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace TEApp.Views.Diary
{
    public class DiaryEntry
    {
        public DateTime Date { get; set; }
        public string Mood { get; set; }
        public List<string> Emotions { get; set; }
        public int EnergyLevel { get; set; }
        public int AnxietyLevel { get; set; }
        public string Events { get; set; }
        public string Thoughts { get; set; }
        public string Gratitude { get; set; }
    }

    public partial class Diary : ContentPage
    {
        private string _selectedMood;
        private List<string> _selectedEmotions;
        private List<DiaryEntry> _entries;
        private Dictionary<string, Frame> _emotionFrames;
        private string[] _motivationalMessages;
        private Random _random;

        private readonly string[] _availableEmotions = new[]
        {
            "😊 Feliz", "😔 Triste", "😰 Ansioso", "😌 Calmo",
            "😤 Irritado", "😨 Com medo", "🥰 Amado", "😞 Desanimado",
            "😎 Confiante", "😓 Estressado", "🤗 Acolhido", "😟 Preocupado",
            "🥳 Animado", "😴 Cansado", "🤔 Confuso", "💪 Forte"
        };

        public Diary()
        {
            InitializeComponent();
            InitializeData();
            LoadEmotions();
            UpdateDateLabel();
            ShowRandomMotivationalMessage();
        }

        private void InitializeData()
        {
            _selectedEmotions = new List<string>();
            _entries = new List<DiaryEntry>();
            _emotionFrames = new Dictionary<string, Frame>();
            _random = new Random();

            _motivationalMessages = new[]
            {
                "Cada dia é uma nova oportunidade para crescer e se conhecer melhor!",
                "Registrar suas emoções é um ato de amor próprio. Continue assim!",
                "Você está fazendo um trabalho incrível de autocuidado!",
                "Suas emoções são válidas. Obrigado por compartilhá-las!",
                "O autoconhecimento é o primeiro passo para o bem-estar!",
                "Você é corajoso por explorar seus sentimentos!",
                "Cada registro é um passo na sua jornada de crescimento!",
                "Suas reflexões são importantes e valiosas!"
            };
        }

        private void UpdateDateLabel()
        {
            var today = DateTime.Now;
            var monthNames = new[] { "", "janeiro", "fevereiro", "março", "abril", "maio", "junho",
                                    "julho", "agosto", "setembro", "outubro", "novembro", "dezembro" };

            var dayOfWeek = today.DayOfWeek switch
            {
                DayOfWeek.Sunday => "Domingo",
                DayOfWeek.Monday => "Segunda-feira",
                DayOfWeek.Tuesday => "Terça-feira",
                DayOfWeek.Wednesday => "Quarta-feira",
                DayOfWeek.Thursday => "Quinta-feira",
                DayOfWeek.Friday => "Sexta-feira",
                DayOfWeek.Saturday => "Sábado",
                _ => ""
            };

            DateLabel.Text = $"{dayOfWeek}, {today.Day} de {monthNames[today.Month]}";
        }

        private void ShowRandomMotivationalMessage()
        {
            MotivationalLabel.Text = _motivationalMessages[_random.Next(_motivationalMessages.Length)];
        }

        private void LoadEmotions()
        {
            EmotionsContainer.Children.Clear();
            _emotionFrames.Clear();

            foreach (var emotion in _availableEmotions)
            {
                var frame = new Frame
                {
                    BackgroundColor = Colors.White,
                    BorderColor = Color.FromArgb("#E0E0E0"),
                    CornerRadius = 20,
                    Padding = new Thickness(12, 8),
                    HasShadow = false,
                    Margin = new Thickness(5)
                };

                var label = new Label
                {
                    Text = emotion,
                    FontSize = 13,
                    TextColor = Color.FromArgb("#666"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                frame.Content = label;

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OnEmotionClicked(emotion, frame, label);
                frame.GestureRecognizers.Add(tapGesture);

                EmotionsContainer.Children.Add(frame);
                _emotionFrames[emotion] = frame;
            }
        }

        private async void OnEmotionClicked(string emotion, Frame frame, Label label)
        {
            if (_selectedEmotions.Contains(emotion))
            {
                // Desseleciona
                _selectedEmotions.Remove(emotion);
                frame.BackgroundColor = Colors.White;
                frame.BorderColor = Color.FromArgb("#E0E0E0");
                label.TextColor = Color.FromArgb("#666");
            }
            else
            {
                // Seleciona
                _selectedEmotions.Add(emotion);
                frame.BackgroundColor = Color.FromArgb("#F3E8FF");
                frame.BorderColor = Color.FromArgb("#8B5CF6");
                label.TextColor = Color.FromArgb("#8B5CF6");
            }

            await frame.ScaleTo(0.95, 50);
            await frame.ScaleTo(1, 50);
        }

        private async void OnMoodVeryHappyClicked(object sender, EventArgs e)
        {
            await SelectMood("Muito Feliz", MoodVeryHappyFrame, "😄", "#4CAF50");
        }

        private async void OnMoodHappyClicked(object sender, EventArgs e)
        {
            await SelectMood("Feliz", MoodHappyFrame, "🙂", "#8BC34A");
        }

        private async void OnMoodNeutralClicked(object sender, EventArgs e)
        {
            await SelectMood("Neutro", MoodNeutralFrame, "😐", "#FFC107");
        }

        private async void OnMoodSadClicked(object sender, EventArgs e)
        {
            await SelectMood("Triste", MoodSadFrame, "😔", "#FF9800");
        }

        private async void OnMoodVerySadClicked(object sender, EventArgs e)
        {
            await SelectMood("Muito Triste", MoodVeryaSadFrame, "😢", "#FF6B6B");
        }

        private async Task SelectMood(string mood, Frame selectedFrame, string emoji, string color)
        {
            _selectedMood = mood;

            // Reset todos os frames
            var frames = new[] { MoodVeryHappyFrame, MoodHappyFrame, MoodNeutralFrame, MoodSadFrame, MoodVeryaSadFrame };
            foreach (var frame in frames)
            {
                frame.BackgroundColor = Colors.White;
                frame.BorderColor = Color.FromArgb("#E0E0E0");
            }

            // Seleciona o frame clicado
            selectedFrame.BackgroundColor = Color.FromArgb(color + "20"); // 20 = transparência
            selectedFrame.BorderColor = Color.FromArgb(color);

            // Animação
            await selectedFrame.ScaleTo(1.1, 100);
            await selectedFrame.ScaleTo(1, 100);
        }

        private void OnEnergyChanged(object sender, ValueChangedEventArgs e)
        {
            EnergyLabel.Text = $"{(int)e.NewValue}%";
        }

        private void OnAnxietyChanged(object sender, ValueChangedEventArgs e)
        {
            AnxietyLabel.Text = $"{(int)e.NewValue}%";
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(_selectedMood))
            {
                await DisplayAlert("Atenção", "Por favor, selecione como você está se sentindo.", "OK");
                return;
            }

            if (_selectedEmotions.Count == 0)
            {
                bool continueWithoutEmotions = await DisplayAlert(
                    "Atenção",
                    "Você não selecionou nenhuma emoção. Deseja continuar mesmo assim?",
                    "Sim",
                    "Não");

                if (!continueWithoutEmotions)
                    return;
            }

            // Cria o registro
            var entry = new DiaryEntry
            {
                Date = DateTime.Now,
                Mood = _selectedMood,
                Emotions = new List<string>(_selectedEmotions),
                EnergyLevel = (int)EnergySlider.Value,
                AnxietyLevel = (int)AnxietySlider.Value,
                Events = EventsEditor.Text ?? "",
                Thoughts = ThoughtsEditor.Text ?? "",
                Gratitude = GratitudeEditor.Text ?? ""
            };

            _entries.Add(entry);

            // Animação do botão
            await SaveButton.ScaleTo(0.95, 50);
            await SaveButton.ScaleTo(1, 50);

            // Mensagem de sucesso
            await DisplayAlert(
                "✅ Registro Salvo!",
                $"Seu diário de {entry.Date:dd/MM/yyyy HH:mm} foi salvo com sucesso!\n\n" +
                $"Continue registrando suas emoções para acompanhar sua jornada! 💚",
                "OK");

            // Limpa o formulário
            ResetForm();
        }

        private void ResetForm()
        {
            // Reset humor
            _selectedMood = null;
            var frames = new[] { MoodVeryHappyFrame, MoodHappyFrame, MoodNeutralFrame, MoodSadFrame, MoodVeryaSadFrame };
            foreach (var frame in frames)
            {
                frame.BackgroundColor = Colors.White;
                frame.BorderColor = Color.FromArgb("#E0E0E0");
            }

            // Reset emoções
            _selectedEmotions.Clear();
            foreach (var kvp in _emotionFrames)
            {
                kvp.Value.BackgroundColor = Colors.White;
                kvp.Value.BorderColor = Color.FromArgb("#E0E0E0");
                if (kvp.Value.Content is Label label)
                {
                    label.TextColor = Color.FromArgb("#666");
                }
            }

            // Reset sliders
            EnergySlider.Value = 50;
            AnxietySlider.Value = 25;

            // Reset editores
            EventsEditor.Text = "";
            ThoughtsEditor.Text = "";
            GratitudeEditor.Text = "";

            // Nova mensagem motivacional
            ShowRandomMotivationalMessage();
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            if (_entries.Count == 0)
            {
                await DisplayAlert(
                    "📖 Histórico",
                    "Você ainda não tem registros salvos.\n\nComece registrando como você está se sentindo hoje!",
                    "OK");
                return;
            }

            var lastEntry = _entries.Last();
            var emotionsText = lastEntry.Emotions.Count > 0
                ? string.Join(", ", lastEntry.Emotions)
                : "Nenhuma emoção registrada";

            await DisplayAlert(
                "📖 Último Registro",
                $"Data: {lastEntry.Date:dd/MM/yyyy HH:mm}\n\n" +
                $"Humor: {lastEntry.Mood}\n" +
                $"Emoções: {emotionsText}\n" +
                $"Energia: {lastEntry.EnergyLevel}%\n" +
                $"Ansiedade: {lastEntry.AnxietyLevel}%\n\n" +
                $"Total de registros: {_entries.Count}",
                "OK");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            bool hasUnsavedData = !string.IsNullOrWhiteSpace(_selectedMood) ||
                                 _selectedEmotions.Count > 0 ||
                                 !string.IsNullOrWhiteSpace(EventsEditor.Text) ||
                                 !string.IsNullOrWhiteSpace(ThoughtsEditor.Text) ||
                                 !string.IsNullOrWhiteSpace(GratitudeEditor.Text);

            if (hasUnsavedData)
            {
                bool leave = await DisplayAlert(
                    "Atenção",
                    "Você tem dados não salvos. Deseja realmente sair?",
                    "Sim",
                    "Não");

                if (!leave)
                    return;
            }

            await Navigation.PopAsync();
        }
    }
}