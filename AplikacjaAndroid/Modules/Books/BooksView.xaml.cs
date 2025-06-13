namespace AplikacjaAndroid;

public partial class BooksView : ContentPage
{
	public BooksView(BooksContext booksContext)
	{
		InitializeComponent();
		BindingContext = booksContext;
    }
}