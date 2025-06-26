namespace AplikacjaAndroid;

public partial class UserConfigView : ContentPage
{
	public UserConfigView(UserConfigContext vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}