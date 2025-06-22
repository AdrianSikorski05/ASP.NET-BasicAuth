namespace AplikacjaAndroid;


public partial class DetailsBookView : ContentPage
{
    public DetailsBookView(DetailsBookContext context)
    {
        InitializeComponent();     
        BindingContext = context;
    }     
}