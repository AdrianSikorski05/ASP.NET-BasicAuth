using Mopups.Pages;

namespace AplikacjaAndroid;

public partial class RefreshTokenPopupView : PopupPage
{
	public RefreshTokenPopupView(RefreshTokenPopupContext vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}