namespace WApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // התחל תמיד במסך התחברות
            return new Window(new NavigationPage(new LoginPage()));
        }
    }
}