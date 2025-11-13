using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace TEApp
{
    public partial class HomePage : ContentPage
    {
        private string nomeUsuario;
        private string primeiroNome;
        private string emailUsuario;

        public HomePage()
        {
            InitializeComponent();
            CarregarDadosUsuario();
        }

        private void CarregarDadosUsuario()
        {
            // IMPORTANTE: Pega o email do usuário logado
            emailUsuario = Preferences.Get("EmailLogado", "");

            if (string.IsNullOrEmpty(emailUsuario))
            {
                // Se não tem email logado, usa valores padrão
                nomeUsuario = "Nome Sobrenome";
                primeiroNome = "Nome";
            }
            else
            {
                // Carrega os dados ESPECÍFICOS deste usuário usando o email como chave
                nomeUsuario = Preferences.Get($"{emailUsuario}_NomeCompleto", "Nome Sobrenome");
                primeiroNome = Preferences.Get($"{emailUsuario}_PrimeiroNome", "Nome");
            }

            // Atualiza as labels
            var labelNomeCompleto = this.FindByName<Label>("LabelNomeCompleto");
            if (labelNomeCompleto != null)
            {
                labelNomeCompleto.Text = nomeUsuario;
            }

            var labelSaudacao = this.FindByName<Label>("LabelSaudacao");
            if (labelSaudacao != null)
            {
                labelSaudacao.Text = $"Olá, {primeiroNome}!";
            }

            // Debug - remove depois de testar
            System.Diagnostics.Debug.WriteLine($"📧 Email logado: {emailUsuario}");
            System.Diagnostics.Debug.WriteLine($"👤 Nome: {nomeUsuario}");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Recarrega os dados quando a página aparecer
            CarregarDadosUsuario();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "Sair da conta",
                "Cancelar",
                "Logout",
                "Ver dados da conta atual"
            );

            if (action == "Logout")
            {
                bool confirmLogout = await DisplayAlert(
                    "Confirmar Logout",
                    $"Deseja realmente sair da conta de {primeiroNome}?",
                    "Sim, sair",
                    "Cancelar"
                );

                if (confirmLogout)
                {
                    try
                    {
                        // Remove o email logado (mantém os dados salvos)
                        Preferences.Remove("EmailLogado");

                        await DisplayAlert("Logout", "Você saiu da sua conta com sucesso!", "OK");

                        // Volta para a tela de login
                        await Navigation.PopToRootAsync();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Erro", $"Erro ao fazer logout: {ex.Message}", "OK");
                    }
                }
            }
            else if (action == "Ver dados da conta atual")
            {
                await DisplayAlert(
                    "Conta Atual",
                    $"👤 Nome: {nomeUsuario}\n📧 Email: {emailUsuario}",
                    "OK"
                );
            }
        }

        private async void OnNotificationClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Notificações", "Você não tem novas notificações", "OK");
        }

        private async void OnPausasSensoriaisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.TimeForRest.NewPage1());
        }

        private async void OnRotinaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TEApp.Views.Routine.Routine());
        }

        private async void OnDiarioEmocionalClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TEApp.Views.Diary.Diary());
        }

        private async void OnIniciarAtividadeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TEApp.Views.Walking.TimeForWorkout());
        }

        private async void OnHiperfocoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TEApp.Views.Hiperfocus.Hiperfocus());
        }

        private async void OnComportamentosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Registro de Comportamentos", "Navegando para registro de comportamentos...", "OK");
            // await Navigation.PushAsync(new Views.Comportamentos.ComportamentosPage());
        }

        private async void OnConselhoDiaClicked(object sender, EventArgs e)
        {
            string[] conselhos = new[]
            {
                $"{primeiroNome}, lembre-se de fazer pausas regulares durante o dia! 🌟",
                $"Celebre pequenas conquistas, {primeiroNome}, elas importam! 🎉",
                "Respiração profunda pode ajudar em momentos de ansiedade. 🧘",
                "Mantenha sua rotina, ela traz segurança e previsibilidade. 📅",
                $"Não hesite em pedir ajuda quando precisar, {primeiroNome}. 🤝",
                "Cada dia é uma nova oportunidade de aprender sobre você. 💜",
                "Está tudo bem não estar bem todos os dias. 🌈",
                "Suas necessidades sensoriais são válidas e importantes. 🎧"
            };

            Random random = new Random();
            string conselhoAleatorio = conselhos[random.Next(conselhos.Length)];

            await DisplayAlert("Conselho do Dia", conselhoAleatorio, "OK");
        }
    }
}