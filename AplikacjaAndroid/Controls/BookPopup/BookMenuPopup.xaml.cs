using CommunityToolkit.Maui.Views;
using Mopups.Pages;
using Mopups.Services;

namespace AplikacjaAndroid;

public partial class BookMenuPopup :PopupPage
{
    public BookMenuPopup(BookMenuPopupContext bookMenuPopupContext)
	{
		InitializeComponent();
		BindingContext = bookMenuPopupContext;	
    }
}