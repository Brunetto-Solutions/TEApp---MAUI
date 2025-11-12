using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace TEApp.Views.Hiperfocus
{
    public partial class Hiperfocus : ContentPage
    {
        private IDispatcherTimer _timer;
        private TimeSpan _timeRemaining;
        private TimeSpan _totalTime;
        private bool _isRunning;
        private bool _isBreak;
        private int _currentSession;
        private int _totalSessions;
        private int _todaySessions;
        private int _todayMinutes;
        private string _selectedMode;
        private string[] _tips;
        private Random _random;

        public Hiperfocus()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeTips();
            SelectMode("Pomodoro");
            ShowRandomTip();
        }

        private void InitializeTimer()
        {
            _isRunning = false;
            _isBreak = false;
            _currentSession = 1;
            _totalSessions = 4;
            _todaySessions = 0;
            _todayMinutes = 0;
            _random = new Random();

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void InitializeTips()
        {
            _tips = new[]
            {
                "Desligue notificações e deixe água por perto durante a sessão de foco!",
                "Faça pausas regulares para evitar a fadiga mental e manter o foco.",
                "Organize seu espaço antes de começar para minimizar distrações.",
                "Use fones de ouvido com música ambiente ou sons da natureza.",
                "Defina uma meta clara antes de cada sessão de foco.",
                "Alongue-se durante as pausas para relaxar corpo e mente.",
                "Mantenha um bloco de notas próximo para anotar ideias que surgirem.",
                "Respire fundo 3 vezes antes de iniciar para acalmar a mente."
            };
        }

        private void ShowRandomTip()
        {
            TipLabel.Text = _tips[_random.Next(_tips.Length)];
        }

        private void SelectMode(string mode)
        {
            _selectedMode = mode;

            // Reset visual dos modos
            PomodoroModeFrame.BackgroundColor = Colors.White;
            PomodoroModeFrame.BorderColor = Color.FromArgb("#E0E0E0");
            ShortModeFrame.BackgroundColor = Colors.White;
            ShortModeFrame.BorderColor = Color.FromArgb("#E0E0E0");
            LongModeFrame.BackgroundColor = Colors.White;
            LongModeFrame.BorderColor = Color.FromArgb("#E0E0E0");

            // Atualiza labels
            foreach (var child in PomodoroModeFrame.Content is VerticalStackLayout stack1 ? stack1.Children : new List<IView>())
            {
                if (child is Label label) label.TextColor = Color.FromArgb("#666");
            }
            foreach (var child in ShortModeFrame.Content is VerticalStackLayout stack2 ? stack2.Children : new List<IView>())
            {
                if (child is Label label) label.TextColor = Color.FromArgb("#666");
            }
            foreach (var child in LongModeFrame.Content is VerticalStackLayout stack3 ? stack3.Children : new List<IView>())
            {
                if (child is Label label) label.TextColor = Color.FromArgb("#666");
            }

            // Seleciona o modo
            switch (mode)
            {
                case "Pomodoro":
                    _totalTime = TimeSpan.FromMinutes(25);
                    PomodoroModeFrame.BackgroundColor = Color.FromArgb("#F3E8FF");
                    PomodoroModeFrame.BorderColor = Color.FromArgb("#8B5CF6");
                    UpdateModeLabels(PomodoroModeFrame, "#8B5CF6");
                    StatusIconLabel.Text = "🍅";
                    break;
                case "Curto":
                    _totalTime = TimeSpan.FromMinutes(15);
                    ShortModeFrame.BackgroundColor = Color.FromArgb("#FFF4E6");
                    ShortModeFrame.BorderColor = Color.FromArgb("#FF9800");
                    UpdateModeLabels(ShortModeFrame, "#FF9800");
                    StatusIconLabel.Text = "⚡";
                    break;
                case "Longo":
                    _totalTime = TimeSpan.FromMinutes(50);
                    LongModeFrame.BackgroundColor = Color.FromArgb("#E3F2FD");
                    LongModeFrame.BorderColor = Color.FromArgb("#2196F3");
                    UpdateModeLabels(LongModeFrame, "#2196F3");
                    StatusIconLabel.Text = "🎯";
                    break;
            }

            _timeRemaining = _totalTime;
            UpdateTimerDisplay();
        }

        private void UpdateModeLabels(Frame frame, string color)
        {
            if (frame.Content is VerticalStackLayout stack)
            {
                foreach (var child in stack.Children)
                {
                    if (child is Label label && label.FontAttributes == FontAttributes.Bold)
                    {
                        label.TextColor = Color.FromArgb(color);
                    }
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_timeRemaining > TimeSpan.Zero)
            {
                _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                UpdateTimerDisplay();

                // Atualiza cor do ícone conforme o tempo
                if (_timeRemaining.TotalMinutes <= 5 && !_isBreak)
                {
                    StatusIconFrame.BackgroundColor = Color.FromArgb("#FFE5E5");
                }
            }
            else
            {
                OnTimerComplete();
            }
        }

        private async void OnTimerComplete()
        {
            _timer.Stop();
            _isRunning = false;

            if (!_isBreak)
            {
                // Sessão de foco completada
                _todaySessions++;
                _todayMinutes += (int)_totalTime.TotalMinutes;
                UpdateTodayStats();

                await DisplayAlert("🎉 Parabéns!",
                    $"Você completou {_totalTime.TotalMinutes} minutos de foco!\n\nHora de fazer uma pausa!",
                    "OK");

                // Inicia pausa
                if (_currentSession < _totalSessions)
                {
                    StartBreak();
                }
                else
                {
                    // Todas as sessões completadas
                    await DisplayAlert("🏆 Excelente!",
                        "Você completou todas as sessões! Faça uma pausa longa agora.",
                        "OK");
                    ResetSession();
                }
            }
            else
            {
                // Pausa completada
                await DisplayAlert("⏰ Pausa finalizada",
                    "Hora de voltar ao foco!",
                    "OK");
                _currentSession++;
                _isBreak = false;
                ResetSession();
            }
        }

        private void StartBreak()
        {
            _isBreak = true;
            _timeRemaining = TimeSpan.FromMinutes(5);
            _totalTime = _timeRemaining;
            StatusIconLabel.Text = "☕";
            StatusIconFrame.BackgroundColor = Color.FromArgb("#E8F5E9");
            StatusLabel.Text = "Pausa - Relaxe!";
            UpdateTimerDisplay();
            UpdateSessionLabel();

            StartButton.IsVisible = true;
            StartButton.Text = "▶ Iniciar Pausa";
            PauseButton.IsVisible = false;
            StopButton.IsVisible = false;
        }

        private void ResetSession()
        {
            SelectMode(_selectedMode);
            StatusLabel.Text = "Pronto para começar";
            UpdateSessionLabel();

            StartButton.IsVisible = true;
            StartButton.Text = "▶ Iniciar Foco";
            PauseButton.IsVisible = false;
            StopButton.IsVisible = false;
        }

        private void UpdateTimerDisplay()
        {
            TimerLabel.Text = $"{(int)_timeRemaining.TotalMinutes:00}:{_timeRemaining.Seconds:00}";
        }

        private void UpdateSessionLabel()
        {
            if (_isBreak)
            {
                SessionLabel.Text = $"Pausa após sessão {_currentSession}";
            }
            else
            {
                SessionLabel.Text = $"Sessão {_currentSession} de {_totalSessions}";
            }
        }

        private void UpdateTodayStats()
        {
            TodaySessionsLabel.Text = _todaySessions.ToString();
            TodayTimeLabel.Text = $"{_todayMinutes} min";
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskEntry.Text) && !_isBreak)
            {
                await DisplayAlert("Atenção", "Digite no que você vai focar antes de começar!", "OK");
                return;
            }

            _isRunning = true;
            _timer.Start();

            StatusLabel.Text = _isBreak ? "Em pausa..." : "Focando...";
            StatusIconFrame.BackgroundColor = _isBreak ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#F0EBFF");

            StartButton.IsVisible = false;
            PauseButton.IsVisible = true;
            StopButton.IsVisible = true;
            TaskInputFrame.IsVisible = false;

            await AnimateButton((Button)sender);
        }

        private async void OnPauseClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                _timer.Stop();
                _isRunning = false;
                PauseButton.Text = "▶ Retomar";
                PauseButton.BackgroundColor = Color.FromArgb("#4CAF50");
                StatusLabel.Text = "Pausado";
            }
            else
            {
                _timer.Start();
                _isRunning = true;
                PauseButton.Text = "⏸ Pausar";
                PauseButton.BackgroundColor = Color.FromArgb("#FF9800");
                StatusLabel.Text = _isBreak ? "Em pausa..." : "Focando...";
            }

            await AnimateButton((Button)sender);
        }

        private async void OnStopClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Parar sessão?",
                "Tem certeza que deseja parar esta sessão?",
                "Sim",
                "Não");

            if (confirm)
            {
                _timer.Stop();
                _isRunning = false;
                _isBreak = false;
                ResetSession();
                TaskInputFrame.IsVisible = true;
                ShowRandomTip();
            }
        }

        private async void OnPomodoroModeClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                SelectMode("Pomodoro");
                await AnimateFrame(PomodoroModeFrame);
            }
        }

        private async void OnShortModeClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                SelectMode("Curto");
                await AnimateFrame(ShortModeFrame);
            }
        }

        private async void OnLongModeClicked(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                SelectMode("Longo");
                await AnimateFrame(LongModeFrame);
            }
        }

        private async Task AnimateButton(Button button)
        {
            await button.ScaleTo(0.95, 50);
            await button.ScaleTo(1, 50);
        }

        private async Task AnimateFrame(Frame frame)
        {
            await frame.ScaleTo(0.95, 50);
            await frame.ScaleTo(1, 50);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                bool confirm = await DisplayAlert(
                    "Atenção",
                    "Você tem uma sessão em andamento. Deseja realmente sair?",
                    "Sim",
                    "Não");

                if (!confirm)
                    return;

                _timer.Stop();
            }

            await Navigation.PopAsync();
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            await DisplayAlert(
                "📊 Estatísticas",
                $"Hoje:\n" +
                $"✅ {_todaySessions} sessões completadas\n" +
                $"⏱️ {_todayMinutes} minutos focados\n\n" +
                $"Continue assim! 💪",
                "OK");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Stop();
        }
    }
}