using Firebase.Auth;
using Microsoft.Maui.Controls;
using TEApp.Views.Login;
using TEApp.Views.InitialScreen;

namespace TEApp.Views.Register
{
    public partial class RegisterPage : ContentPage
    {
        private readonly FirebaseAuthClient _authClient;

        public RegisterPage(FirebaseAuthClient authClient)
        {
            InitializeComponent();
            _authClient = authClient;

            // Associa o botão Cadastrar à função
            var registerButton = this.FindByName<Button>("CadastrarButton");
            if (registerButton != null)
                registerButton.Clicked += OnRegisterClicked;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Pegando os valores dos Entry pelo nome ou index
            var nameEntry = this.FindByName<Entry>("NomeEntry");
            var emailEntry = this.FindByName<Entry>("EmailEntry");
            var passwordEntry = this.FindByName<Entry>("PasswordEntry");
            var confirmEntry = this.FindByName<Entry>("ConfirmPasswordEntry");

            string name = nameEntry?.Text?.Trim();
            string email = emailEntry?.Text?.Trim();
            string password = passwordEntry?.Text;
            string confirmPassword = confirmEntry?.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            try
            {
                var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                if (userCredential.User != null)
                {
                    await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "OK");

                    // Redireciona para tela inicial
                    await Navigation.PushAsync(new InitialScreen.InitialScreen());
                }
            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Erro", $"Falha no cadastro: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            }
        }

        private async void loginRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(_authClient));
        }
    }
}
