namespace AplikacjaAndroid;

public partial class UsersView : ContentPage
{
	public UsersView(UsersContext usersContext)
	{
		InitializeComponent();
		BindingContext = usersContext;
    }
}