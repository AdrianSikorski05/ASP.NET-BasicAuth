using Mopups.Pages;

namespace AplikacjaAndroid;

public partial class CommentPopupView : PopupPage
{
	public CommentPopupView(CommentPopupContext context)
	{
		InitializeComponent();
		BindingContext = context;
	}
}