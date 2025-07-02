using Mopups.Pages;
using System.Threading.Tasks;

namespace AplikacjaAndroid;

public partial class ConfirmeDeleteAccountView : PopupPage
{
	public ConfirmeDeleteAccountView(ConfirmeDeleteAccountContext vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }


}