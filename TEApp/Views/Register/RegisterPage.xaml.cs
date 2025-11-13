using Firebase.Auth;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
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
            // Pegando os valores dos Entry pelo nome
            var nameEntry = this.FindByName<Entry>("NomeEntry");
            var emailEntry = this.FindByName<Entry>("EmailEntry");
            var passwordEntry = this.FindByName<Entry>("PasswordEntry");
            var confirmEntry = this.FindByName<Entry>("ConfirmPasswordEntry");

            string name = nameEntry?.Text?.Trim();
            string email = emailEntry?.Text?.Trim().ToLower(); // SEMPRE em minúsculo
            string password = passwordEntry?.Text;
            string confirmPassword = confirmEntry?.Text;

            // Validações
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (!IsValidEmail(email))
            {
                await DisplayAlert("Erro", "Por favor, insira um email válido.", "OK");
                return;
            }

            if (password.Length < 6)
            {
                await DisplayAlert("Erro", "A senha deve ter no mínimo 6 caracteres.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            try
            {
                // Cria o usuário no Firebase
                var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                if (userCredential.User != null)
                {
                    // ====== SALVA OS DADOS COM PREFIXO DO EMAIL ======
                    SalvarDadosUsuario(name, email);

                    // Extrai o primeiro nome para a mensagem
                    string primeiroNome = name.Split(' ')[0];

                    // Debug - remove depois de testar
                    System.Diagnostics.Debug.WriteLine($"✅ Cadastro realizado:");
                    System.Diagnostics.Debug.WriteLine($"   📧 Email: {email}");
                    System.Diagnostics.Debug.WriteLine($"   👤 Nome: {name}");
                    System.Diagnostics.Debug.WriteLine($"   👋 Primeiro Nome: {primeiroNome}");
                    System.Diagnostics.Debug.WriteLine($"   🔑 Chave salva: {email}_NomeCompleto");

                    await DisplayAlert("Sucesso", $"Bem-vindo(a), {primeiroNome}! Cadastro realizado com sucesso!", "OK");

                    // Redireciona para tela inicial
                    await Navigation.PushAsync(new InitialScreen.InitialScreen());
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

        private void SalvarDadosUsuario(string nomeCompleto, string email)
        {
            // ====== IMPORTANTE: SALVA COM PREFIXO DO EMAIL ======

            // Define qual usuário está logado
            Preferences.Set("EmailLogado", email);

            // Extrai o primeiro nome
            string primeiroNome = nomeCompleto.Split(' ')[0];

            // Salva o nome completo COM PREFIXO do email
            Preferences.Set($"{email}_NomeCompleto", nomeCompleto);

            // Salva o primeiro nome COM PREFIXO do email
            Preferences.Set($"{email}_PrimeiroNome", primeiroNome);

            // Marca que este usuário específico está cadastrado
            Preferences.Set($"{email}_UsuarioCadastrado", true);

            // Debug - mostra o que foi salvo
            System.Diagnostics.Debug.WriteLine($"💾 Dados salvos para {email}:");
            System.Diagnostics.Debug.WriteLine($"   {email}_NomeCompleto = {nomeCompleto}");
            System.Diagnostics.Debug.WriteLine($"   {email}_PrimeiroNome = {primeiroNome}");
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
                AuthErrorReason.EmailExists => "Este email já está cadastrado.",
                AuthErrorReason.WeakPassword => "Senha muito fraca. Use no mínimo 6 caracteres.",
                AuthErrorReason.InvalidEmailAddress => "Email inválido.",
                AuthErrorReason.MissingPassword => "Por favor, insira uma senha.",
                AuthErrorReason.TooManyAttemptsTryLater => "Muitas tentativas. Tente novamente mais tarde.",
                _ => $"Falha no cadastro: {reason}"
            };
        }

        private async void loginRedirect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(_authClient));
        }
    }
}