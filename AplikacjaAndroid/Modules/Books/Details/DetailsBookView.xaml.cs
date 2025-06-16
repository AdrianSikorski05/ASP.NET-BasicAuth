namespace AplikacjaAndroid;

[QueryProperty(nameof(IsVisibleButtons), "IsVisibleButtons")]
[QueryProperty(nameof(IsVisibleConfig), "IsVisibleConfig")]
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


    private bool _isVisibleConfig;
    public bool IsVisibleConfig
    {
        get => _isVisibleConfig;
        set
        {
            _isVisibleConfig = value;
            _context.IsVisibleConfig = value;
        }
    }

    private bool _isVisibleButtons;
    public bool IsVisibleButtons
    {
        get => _isVisibleButtons;
        set
        {
            _isVisibleButtons = value;
            _context.IsVisibleButtons = value;
        }
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