using Mopups.Pages;

namespace AplikacjaAndroid;

public partial class CommentPopupView : PopupPage
{
	public CommentPopupView(CommentPopupContext context)
	{
		InitializeComponent();
		BindingContext = context;
	}

    protected override bool OnBackgroundClicked()
    {
        if (BindingContext is CommentPopupContext context)
            context.ReturnResult(null); 
        return base.OnBackgroundClicked();
    }

}