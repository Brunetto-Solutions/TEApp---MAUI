namespace TEApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routes
            Routing.RegisterRoute("initialScreen", typeof(Views.InitialScreen.InitialScreen));
            Routing.RegisterRoute("loginPage", typeof(Views.Login.LoginPage));
            Routing.RegisterRoute("HomeScreen", typeof(HomePage));
            Routing.RegisterRoute("TherapeuticScreen", typeof(Views.TherapeuticScreen.TherapeuticScreen));
            Routing.RegisterRoute("TriageScreen", typeof(Views.TriageScreen.TriageScreen));
        }
    }
}
