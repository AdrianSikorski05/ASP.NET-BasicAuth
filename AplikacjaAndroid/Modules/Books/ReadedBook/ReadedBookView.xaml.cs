namespace AplikacjaAndroid;

public partial class ReadedBookView : ContentPage
{
	public ReadedBookView(ReadedBookContext readedBookContext)
	{
		InitializeComponent();
		BindingContext = readedBookContext;
    }
}