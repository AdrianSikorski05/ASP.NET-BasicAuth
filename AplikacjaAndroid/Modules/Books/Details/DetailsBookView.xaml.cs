namespace AplikacjaAndroid;

[QueryProperty(nameof(Book), "Book")]
public partial class DetailsBookView : ContentPage
{
    private readonly DetailsBookContext _context;

    public DetailsBookView(DetailsBookContext context)
    {
        InitializeComponent();
        _context = context;
        BindingContext = _context;
    }

    private Book _book;
    public Book Book
    {
        get => _book;
        set
        {
            _book = value;
            _context.Book = value;
        }
    }
}