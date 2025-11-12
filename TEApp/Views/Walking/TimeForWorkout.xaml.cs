using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace TEApp.Views.Walking
{
    public partial class TimeForWorkout : ContentPage
    {
        private IDispatcherTimer _timer;
        private TimeSpan _elapsedTime;
        private bool _isRunning;
        private double _distance;
        private int _calories;
        private int _steps;
        private readonly Random _random;

        public TimeForWorkout()
        {
            InitializeComponent();
            _random = new Random();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _elapsedTime = TimeSpan.Zero;
            _isRunning = false;
            _distance = 0;
            _calories = 0;
            _steps = 0;

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();
            UpdateStats();
            UpdateMotivationalMessage();
        }

        private void UpdateTimerDisplay()
        {
            TimerLabel.Text = _elapsedTime.ToString(@"mm\:ss");
        }

        private void UpdateStats()
        {
            // Simulação realista de caminhada
            // Em uma implementação real, você usaria GPS e sensores

            // Incrementa passos (aproximadamente 2 passos por segundo)
            _steps += _random.Next(1, 3);

            // Calcula distância (velocidade média de caminhada: ~5 km/h)
            _distance = (_steps * 0.0007); // aproximadamente 70cm por passo

            // Calcula calorias (aproximadamente 3-4 kcal por minuto de caminhada)
            _calories = (int)(_elapsedTime.TotalMinutes * 3.5);

            // Atualiza UI
            DistanceLabel.Text = $"{_distance:F2} km";
            CaloriesLabel.Text = $"{_calories} kcal";
            StepsLabel.Text = _steps.ToString();
        }

        private void UpdateMotivationalMessage()
        {
            string[] messages = {
                "Você está indo muito bem! Continue assim! 💪",
                "Ótimo ritmo! Mantenha o foco! 🎯",
                "Cada passo conta! Você é incrível! ⭐",
                "Continue em frente! Você consegue! 🚀",
                "Excelente progresso! Siga firme! 🌟",
                "Sua dedicação é inspiradora! 💫",
                "Você está arrasando! Continue! 🔥"
            };

            int minutes = (int)_elapsedTime.TotalMinutes;
            if (minutes > 0 && _elapsedTime.Seconds == 0)
            {
                MotivationalLabel.Text = messages[_random.Next(messages.Length)];
            }
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            _isRunning = true;
            _timer.Start();

            // Atualiza visibilidade dos botões
            StartButton.IsVisible = false;
            PauseButton.IsVisible = true;
            StopButton.IsVisible = true;

            await AnimateButton((Button)sender);
        }

        private async void OnPauseClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                // Pausar
                _timer.Stop();
                _isRunning = false;
                PauseButton.Text = "▶ Retomar";
                PauseButton.BackgroundColor = Color.FromArgb("#4CAF50");
            }
            else
            {
                // Retomar
                _timer.Start();
                _isRunning = true;
                PauseButton.Text = "⏸ Pausar";
                PauseButton.BackgroundColor = Color.FromArgb("#8B5CF6");
            }

            await AnimateButton((Button)sender);
        }

        private async void OnStopClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Finalizar Atividade",
                $"Você completou {_elapsedTime:mm\\:ss} de caminhada!\n\n" +
                $"📍 Distância: {_distance:F2} km\n" +
                $"🔥 Calorias: {_calories} kcal\n" +
                $"👟 Passos: {_steps}\n\n" +
                "Deseja finalizar esta atividade?",
                "Sim",
                "Continuar");

            if (confirm)
            {
                _timer.Stop();
                _isRunning = false;

                // Oculta os botões de controle e mostra mensagem de conclusão
                StartButton.IsVisible = false;
                PauseButton.IsVisible = false;
                StopButton.IsVisible = false;

                // Atualiza a mensagem motivacional para mensagem de parabéns
                MotivationalLabel.Text = "🎉 Parabéns! Atividade concluída com sucesso! Você está no caminho certo! 💪";
            }
            else
            {
                // Se escolher continuar, retoma o timer se estava rodando antes de pausar
                if (_isRunning)
                {
                    _timer.Start();
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                bool confirm = await DisplayAlert(
                    "Atenção",
                    "Você tem uma atividade em andamento. Deseja realmente sair?",
                    "Sim",
                    "Não");

                if (!confirm)
                    return;

                _timer.Stop();
            }

            await Navigation.PopAsync();
        }

        private async Task AnimateButton(Button button)
        {
            await button.ScaleTo(0.95, 50);
            await button.ScaleTo(1, 50);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Stop();
        }
    }
}