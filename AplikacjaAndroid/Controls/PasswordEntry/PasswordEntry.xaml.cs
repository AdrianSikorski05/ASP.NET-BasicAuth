using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace AplikacjaAndroid;

public partial class PasswordEntry : ContentView
{
    public PasswordEntry()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(PasswordEntry), default(string), BindingMode.TwoWay, propertyChanged: OnTextChanged);


    public static readonly BindableProperty FontSizeProperty =
    BindableProperty.Create(nameof(FontSize), typeof(double), typeof(PasswordEntry), 17.0);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set
        {
            SetValue(TextProperty, value);
            OnPropertyChanged();
        }
    }


    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PasswordEntry), default(string));

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty IsPasswordHiddenProperty =
        BindableProperty.Create(nameof(IsPasswordHidden), typeof(bool), typeof(PasswordEntry), true, BindingMode.TwoWay);

    public bool IsPasswordHidden
    {
        get => (bool)GetValue(IsPasswordHiddenProperty);
        set
        {
            SetValue(IsPasswordHiddenProperty, value);
            OnPropertyChanged(nameof(EyeIconGlyph));
        }
    }

    public static readonly BindableProperty IsFocusedProperty =
        BindableProperty.Create(nameof(IsFocused), typeof(bool), typeof(PasswordEntry), false, BindingMode.TwoWay);

    public bool IsFocused
    {
        get => (bool)GetValue(IsFocusedProperty);
        set => SetValue(IsFocusedProperty, value);
    }

    // Usuń BindableProperty dla IsEyeVisible - będzie to computed property
    public bool IsEyeVisible => IsFocused && !string.IsNullOrWhiteSpace(Text);

    public string EyeIconGlyph => IsPasswordHidden ? "\ue8f5" : "\ue8f4"; // MaterialIcons visibility/visibility_off

    public ICommand ToggleVisibilityCommand => new Command(() =>
    {
        IsPasswordHidden = !IsPasswordHidden;
    });

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PasswordEntry control)
        {
            control.OnPropertyChanged(nameof(control.IsEyeVisible));
        }
    }

    // Event handlery
    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        IsFocused = true;
        OnPropertyChanged(nameof(IsEyeVisible));
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        IsFocused = false;
        OnPropertyChanged(nameof(IsEyeVisible));
    }


}