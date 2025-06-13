namespace AplikacjaAndroid
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public App(LoginView loginView, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = loginView;
            ServiceProvider = serviceProvider;
        }
    }
}