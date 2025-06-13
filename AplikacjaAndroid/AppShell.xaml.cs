namespace AplikacjaAndroid
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellContext context)
        {
            InitializeComponent();
            BindingContext = context;

            // Rejestruj routes dla nawigacji
            Routing.RegisterRoute(nameof(BooksView), typeof(BooksView));
            Routing.RegisterRoute(nameof(UsersView), typeof(UsersView));
            Routing.RegisterRoute(nameof(DetailsBookView), typeof(DetailsBookView));
            Routing.RegisterRoute(nameof(ToReadBookView), typeof(ToReadBookView));
            Routing.RegisterRoute(nameof(ReadedBookView), typeof(ReadedBookView));
        }
    }
}
