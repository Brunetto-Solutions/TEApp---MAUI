using Firebase.Auth;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Verifica se o usuário já está logado
            await VerificarUsuarioLogado();
        }

        private async Task VerificarUsuarioLogado()
        {
            try
            {
                // Verifica se há um usuário autenticado no Firebase
                var user = _authClient.User;

                // Também verifica se há dados salvos localmente
                bool usuarioCadastrado = Preferences.Get("UsuarioCadastrado", false);

                if (user != null && usuarioCadastrado)
                {
                    // Usuário já está logado, vai direto para a HomePage
                    await Navigation.PushAsync(new InitialScreen.InitialScreen());
                }
            }
            catch (Exception ex)
            {
                // Se houver erro, continua na tela de login
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar usuário: {ex.Message}");
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Obtém o email e a senha digitados pelo usuário
            string email = emailEntry.Text?.Trim();
            string password = passwordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (!IsValidEmail(email))
            {
                await DisplayAlert("Erro", "Por favor, insira um email válido.", "OK");
                return;
            }

            try
            {
                // Tenta autenticar o usuário com o Firebase
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Se o login for bem-sucedido
                if (userCredential.User != null)
                {
                    // Salva o email do usuário
                    Preferences.Set("Email", email);
                    Preferences.Set("UsuarioCadastrado", true);

                    // Se não tiver nome salvo, tenta buscar do Firebase ou usa o email
                    string nomeCompleto = Preferences.Get("NomeCompleto", "");
                    if (string.IsNullOrWhiteSpace(nomeCompleto))
                    {
                        // Usa o nome do email como fallback
                        string nomeFallback = email.Split('@')[0];
                        Preferences.Set("NomeCompleto", nomeFallback);
                        Preferences.Set("PrimeiroNome", nomeFallback);
                    }

                    await DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");

                    // Navega para a tela inicial
                    await Navigation.PushAsync(new InitialScreen.InitialScreen());
                }
                else
                {
                    await DisplayAlert("Erro", "Login falhou. Por favor, tente novamente.", "OK");
                }
            }
            catch (FirebaseAuthException ex)
            {
                string mensagemErro = ObterMensagemErro(ex.Reason);
                await DisplayAlert("Erro", mensagemErro, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string ObterMensagemErro(AuthErrorReason reason)
        {
            return reason switch
            {
                AuthErrorReason.InvalidEmailAddress => "Email inválido.",
                AuthErrorReason.WrongPassword => "Senha incorreta.",
                AuthErrorReason.UserNotFound => "Usuário não encontrado.",
                AuthErrorReason.UserDisabled => "Esta conta foi desativada.",
                AuthErrorReason.TooManyAttemptsTryLater => "Muitas tentativas. Tente novamente mais tarde.",
                _ => "Credenciais incorretas. Verifique seu email e senha."
            };
        }

        private async void registerRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register.RegisterPage(_authClient));
        }

        private async void ForgotPasswordRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPassword.ForgotPassword());
        }
    }
}