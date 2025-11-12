using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TEApp.Views.TimeForRest
{
    public partial class NewPage1 : ContentPage
    {
        // Cancellation Tokens para cada timer
        private CancellationTokenSource _cts5;
        private CancellationTokenSource _cts10;
        private CancellationTokenSource _cts15;

        // Tempo restante em segundos
        private int _tempoRestante5 = 300; // 5 minutos
        private int _tempoRestante10 = 600; // 10 minutos
        private int _tempoRestante15 = 900; // 15 minutos

        // Estado dos timers
        private bool _timer5Rodando = false;
        private bool _timer10Rodando = false;
        private bool _timer15Rodando = false;

        public NewPage1()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Para todos os timers ao sair da página
            PararTodosTimers();
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            // Para todos os timers antes de voltar
            PararTodosTimers();
            await Navigation.PopAsync();
        }

        #region Timer 5 Minutos

        private async void OnIniciar5Clicked(object sender, EventArgs e)
        {
            if (_timer5Rodando) return;

            _timer5Rodando = true;
            BtnIniciar5.IsEnabled = false;
            BtnParar5.IsEnabled = true;
            BtnReiniciar5.IsEnabled = true;

            _cts5 = new CancellationTokenSource();
            await ExecutarTimer(5, _tempoRestante5, Timer5Label, _cts5.Token);
        }

        private void OnParar5Clicked(object sender, EventArgs e)
        {
            _timer5Rodando = false;
            _cts5?.Cancel();
            BtnIniciar5.IsEnabled = true;
            BtnParar5.IsEnabled = false;
        }

        private void OnReiniciar5Clicked(object sender, EventArgs e)
        {
            _timer5Rodando = false;
            _cts5?.Cancel();
            _tempoRestante5 = 300;
            Timer5Label.Text = "05:00";
            BtnIniciar5.IsEnabled = true;
            BtnParar5.IsEnabled = false;
            BtnReiniciar5.IsEnabled = false;
        }

        #endregion

        #region Timer 10 Minutos

        private async void OnIniciar10Clicked(object sender, EventArgs e)
        {
            if (_timer10Rodando) return;

            _timer10Rodando = true;
            BtnIniciar10.IsEnabled = false;
            BtnParar10.IsEnabled = true;
            BtnReiniciar10.IsEnabled = true;

            _cts10 = new CancellationTokenSource();
            await ExecutarTimer(10, _tempoRestante10, Timer10Label, _cts10.Token);
        }

        private void OnParar10Clicked(object sender, EventArgs e)
        {
            _timer10Rodando = false;
            _cts10?.Cancel();
            BtnIniciar10.IsEnabled = true;
            BtnParar10.IsEnabled = false;
        }

        private void OnReiniciar10Clicked(object sender, EventArgs e)
        {
            _timer10Rodando = false;
            _cts10?.Cancel();
            _tempoRestante10 = 600;
            Timer10Label.Text = "10:00";
            BtnIniciar10.IsEnabled = true;
            BtnParar10.IsEnabled = false;
            BtnReiniciar10.IsEnabled = false;
        }

        #endregion

        #region Timer 15 Minutos

        private async void OnIniciar15Clicked(object sender, EventArgs e)
        {
            if (_timer15Rodando) return;

            _timer15Rodando = true;
            BtnIniciar15.IsEnabled = false;
            BtnParar15.IsEnabled = true;
            BtnReiniciar15.IsEnabled = true;

            _cts15 = new CancellationTokenSource();
            await ExecutarTimer(15, _tempoRestante15, Timer15Label, _cts15.Token);
        }

        private void OnParar15Clicked(object sender, EventArgs e)
        {
            _timer15Rodando = false;
            _cts15?.Cancel();
            BtnIniciar15.IsEnabled = true;
            BtnParar15.IsEnabled = false;
        }

        private void OnReiniciar15Clicked(object sender, EventArgs e)
        {
            _timer15Rodando = false;
            _cts15?.Cancel();
            _tempoRestante15 = 900;
            Timer15Label.Text = "15:00";
            BtnIniciar15.IsEnabled = true;
            BtnParar15.IsEnabled = false;
            BtnReiniciar15.IsEnabled = false;
        }

        #endregion

        #region Lógica Comum dos Timers

        private async Task ExecutarTimer(int timerNum, int tempoInicial, Label label, CancellationToken token)
        {
            int tempoRestante = tempoInicial;

            try
            {
                while (tempoRestante > 0 && !token.IsCancellationRequested)
                {
                    // Atualiza o display
                    int minutos = tempoRestante / 60;
                    int segundos = tempoRestante % 60;

                    // Precisa atualizar na thread principal
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        label.Text = $"{minutos:D2}:{segundos:D2}";
                    });

                    // Aguarda 1 segundo
                    await Task.Delay(1000, token);
                    tempoRestante--;

                    // Atualiza o tempo restante na variável correspondente
                    if (timerNum == 5) _tempoRestante5 = tempoRestante;
                    else if (timerNum == 10) _tempoRestante10 = tempoRestante;
                    else if (timerNum == 15) _tempoRestante15 = tempoRestante;
                }

                // Timer chegou ao fim
                if (tempoRestante == 0 && !token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await TimerConcluido(timerNum);
                    });
                }
            }
            catch (TaskCanceledException)
            {
                // Timer foi cancelado, não faz nada
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Erro", $"Erro no timer: {ex.Message}", "OK");
                });
            }
        }

        private async Task TimerConcluido(int timerNum)
        {
            // Toca som ou vibração (opcional)
            try
            {
                // Vibração do dispositivo
                var duration = TimeSpan.FromSeconds(1);
                Vibration.Default.Vibrate(duration);
            }
            catch
            {
                // Dispositivo não suporta vibração
            }

            // Mensagem de conclusão
            string mensagem = timerNum switch
            {
                5 => "Sua pausa de 5 minutos terminou! 🎉\nEspero que tenha se sentido melhor.",
                10 => "Sua pausa de 10 minutos terminou! 🎉\nContinue cuidando de você.",
                15 => "Sua pausa de 15 minutos terminou! 🎉\nÓtimo trabalho em priorizar seu bem-estar.",
                _ => "Timer concluído!"
            };

            await DisplayAlert("Pausa Concluída", mensagem, "OK");

            // Reinicia o timer correspondente
            if (timerNum == 5)
            {
                _timer5Rodando = false;
                _tempoRestante5 = 300;
                Timer5Label.Text = "05:00";
                BtnIniciar5.IsEnabled = true;
                BtnParar5.IsEnabled = false;
                BtnReiniciar5.IsEnabled = false;
            }
            else if (timerNum == 10)
            {
                _timer10Rodando = false;
                _tempoRestante10 = 600;
                Timer10Label.Text = "10:00";
                BtnIniciar10.IsEnabled = true;
                BtnParar10.IsEnabled = false;
                BtnReiniciar10.IsEnabled = false;
            }
            else if (timerNum == 15)
            {
                _timer15Rodando = false;
                _tempoRestante15 = 900;
                Timer15Label.Text = "15:00";
                BtnIniciar15.IsEnabled = true;
                BtnParar15.IsEnabled = false;
                BtnReiniciar15.IsEnabled = false;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void PararTodosTimers()
        {
            // Cancela todos os timers
            _cts5?.Cancel();
            _cts10?.Cancel();
            _cts15?.Cancel();

            // Reseta os estados
            _timer5Rodando = false;
            _timer10Rodando = false;
            _timer15Rodando = false;
        }

        #endregion
    }
}