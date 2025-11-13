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

                // Verifica se há email logado salvo localmente
                string emailLogado = Preferences.Get("EmailLogado", "");

                if (user != null && !string.IsNullOrEmpty(emailLogado))
                {
                    // Verifica se o usuário específico está cadastrado
                    bool usuarioCadastrado = Preferences.Get($"{emailLogado}_UsuarioCadastrado", false);

                    if (usuarioCadastrado)
                    {
                        // Usuário já está logado, vai direto para a HomePage
                        await Navigation.PushAsync(new InitialScreen.InitialScreen());
                    }
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
            string email = emailEntry.Text?.Trim().ToLower(); // SEMPRE em minúsculo
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
                    // ====== IMPORTANTE: SALVA COM PREFIXO DO EMAIL ======

                    // Define qual usuário está logado
                    Preferences.Set("EmailLogado", email);

                    // PRIORIDADE 1: Tenta buscar o nome que foi salvo no CADASTRO
                    string nomeCompleto = Preferences.Get($"{email}_NomeCompleto", "");
                    string primeiroNome = Preferences.Get($"{email}_PrimeiroNome", "");

                    // PRIORIDADE 2: Se não encontrou localmente, busca do Firebase
                    if (string.IsNullOrWhiteSpace(nomeCompleto))
                    {
                        string displayName = userCredential.User.Info?.DisplayName;

                        if (!string.IsNullOrWhiteSpace(displayName))
                        {
                            nomeCompleto = displayName;
                            primeiroNome = displayName.Split(' ')[0];

                            // Salva localmente para próximas vezes
                            Preferences.Set($"{email}_NomeCompleto", nomeCompleto);
                            Preferences.Set($"{email}_PrimeiroNome", primeiroNome);
                        }
                        else
                        {
                            // ÚLTIMA OPÇÃO: Se não tem nada, usa email temporariamente
                            // Isso só acontece se o cadastro falhou em salvar o nome
                            string nomeFallback = email.Split('@')[0];
                            nomeFallback = char.ToUpper(nomeFallback[0]) + nomeFallback.Substring(1);
                            nomeCompleto = nomeFallback;
                            primeiroNome = nomeFallback;

                            System.Diagnostics.Debug.WriteLine($"⚠️ AVISO: Nome não encontrado para {email}");
                            System.Diagnostics.Debug.WriteLine($"   Usando fallback: {nomeFallback}");
                        }
                    }

                    // Marca que este usuário está cadastrado
                    Preferences.Set($"{email}_UsuarioCadastrado", true);

                    // Debug - remove depois de testar
                    System.Diagnostics.Debug.WriteLine($"✅ Login realizado:");
                    System.Diagnostics.Debug.WriteLine($"   📧 Email: {email}");
                    System.Diagnostics.Debug.WriteLine($"   👤 Nome Completo: {nomeCompleto}");
                    System.Diagnostics.Debug.WriteLine($"   👋 Primeiro Nome: {primeiroNome}");
                    System.Diagnostics.Debug.WriteLine($"   🔑 Chave usada: {email}_NomeCompleto");

                    await DisplayAlert("Sucesso", $"Bem-vindo de volta, {primeiroNome}!", "OK");

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