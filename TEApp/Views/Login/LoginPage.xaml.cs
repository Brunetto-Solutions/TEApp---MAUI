using Firebase.Auth;
using Microsoft.Maui.Controls;

namespace TEApp.Views.Login
{
    public partial class LoginPage : ContentPage
    {
        private readonly FirebaseAuthClient _authClient;

        public LoginPage(FirebaseAuthClient authClient)
        {
            InitializeComponent();
            _authClient = authClient;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Obtém o email e a senha digitados pelo usuário
            string email = emailEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            try
            {
                // Tenta autenticar o usuário com o Firebase
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Se o login for bem-sucedido, navegue para a tela inicial
                if (userCredential.User != null)
                {
                    await Navigation.PushAsync(new InitialScreen.InitialScreen());
                }
                else
                {
                    // Isso geralmente não acontece se o método não lançar uma exceção, mas é bom ter
                    await DisplayAlert("Erro", "Login falhou. Por favor, tente novamente.", "OK");
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Se a autenticação falhar, exibe uma mensagem de erro ao usuário
                await DisplayAlert("Erro", "Credencias Incorretas" +
                    "" +
                    "", "OK");
            }
            catch (Exception ex)
            {
                // Captura outros erros inesperados
                await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            }
        }

        private async void registerRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register.RegisterPage());
        }

        private async void ForgotPasswordRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPassword.ForgotPassword());
        }
    }
}
