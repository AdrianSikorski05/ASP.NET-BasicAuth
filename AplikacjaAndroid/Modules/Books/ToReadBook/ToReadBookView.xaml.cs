namespace AplikacjaAndroid;

public partial class ToReadBookView : ContentPage
{
	public ToReadBookView(ToReadBookContext toReadBookContext)
	{
		InitializeComponent();
		BindingContext = toReadBookContext;
	}
}