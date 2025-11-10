using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace TEApp
{
    public partial class HomePage : ContentPage
    {
        private string nomeUsuario;
        private string primeiroNome;

        public HomePage()
        {
            InitializeComponent();
            CarregarDadosUsuario();
        }

        private void CarregarDadosUsuario()
        {
            // Recupera o nome completo das preferências
            nomeUsuario = Preferences.Get("NomeCompleto", "Nome Sobrenome");
            primeiroNome = Preferences.Get("PrimeiroNome", "Nome");

            // Atualiza os Labels na interface
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Recarrega os dados quando a página aparecer
            CarregarDadosUsuario();
        }

        private async void OnLinkClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Link", "Função de compartilhamento em desenvolvimento", "OK");
        }

        private async void OnNotificationClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Notificações", "Você não tem novas notificações", "OK");
        }

        private async void OnPausasSensoriaisClicked(object sender, EventArgs e)
        {
            // Navega para a tela de Pausas Sensoriais (Time For Rest)
            await Navigation.PushAsync(new Views.TimeForRest.NewPage1());
        }

        private async void OnRotinaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Rotina", "Navegando para rotina...", "OK");
            // await Navigation.PushAsync(new Views.Rotina.RotinaPage());
        }

        private async void OnDiarioEmocionalClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Diário Emocional", "Navegando para diário emocional...", "OK");
            // await Navigation.PushAsync(new Views.DiarioEmocional.DiarioEmocionalPage());
        }

        private async void OnIniciarAtividadeClicked(object sender, EventArgs e)
        {
            bool resposta = await DisplayAlert(
                "Iniciar Atividade",
                $"{primeiroNome}, deseja iniciar a caminhada das 15:00?",
                "Sim",
                "Não"
            );

            if (resposta)
            {
                await DisplayAlert("Atividade Iniciada", "Boa caminhada! 🚶‍♂️", "OK");
            }
        }

        private async void OnHiperfocoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Gestão de Hiperfoco", "Navegando para gestão de hiperfoco...", "OK");
            // await Navigation.PushAsync(new Views.Hiperfoco.HiperfocoPage());
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

        private async void OnRelatoriosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Relatórios Inteligentes", "Navegando para relatórios...", "OK");
            // await Navigation.PushAsync(new Views.Relatorios.RelatoriosPage());
        }
    }
}