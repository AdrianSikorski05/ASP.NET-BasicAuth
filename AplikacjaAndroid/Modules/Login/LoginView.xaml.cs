namespace AplikacjaAndroid;

public partial class LoginView : ContentPage
{
	public LoginView(LoginContext loginContext)
	{
		InitializeComponent();
		BindingContext = loginContext;
    }   
}