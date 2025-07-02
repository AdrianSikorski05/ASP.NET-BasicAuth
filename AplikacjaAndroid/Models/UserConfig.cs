using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AplikacjaAndroid
{
    public partial class UserConfig : ObservableValidator
    {

        public UserConfig()
        {
            SelectedColorAsColor = Color.FromArgb(SelectedColor);
        }

        public int? Id { get; set; }
        public int? UserId { get; set; }

        [ObservableProperty]
        string? _name = "New";
        partial void OnNameChanged(string? value)
        {
            OnPropertyChanged(nameof(Initials));
        }

        [ObservableProperty]
        string? _surename = "New";
        partial void OnSurenameChanged(string? value)
        {
            OnPropertyChanged(nameof(Initials));
        }

        [ObservableProperty] 
        string? _email;

        [ObservableProperty]
        string? _phoneNumber;

        [ObservableProperty]
        string? _selectedColor = "#A3D5FF";

        [JsonIgnore]
        [ObservableProperty]
        Color _selectedColorAsColor = Colors.Transparent;
        partial void OnSelectedColorChanged(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    SelectedColorAsColor = Color.FromArgb(value);
                }
                catch
                {
                    // fallback jeśli zły hex
                    SelectedColorAsColor = Colors.Transparent;
                }
            }
        }

        partial void OnSelectedColorAsColorChanged(Color value)
        {
            SelectedColor = value.ToArgbHex();
        }

        [ObservableProperty]
        byte[]? _avatarBytes = Array.Empty<byte>();
        partial void OnAvatarBytesChanged(byte[]? value)
        {
            if (AvatarBytes != null)
            {
                AvatarImage = ImageSource.FromStream(() => new MemoryStream(AvatarBytes));
            }
        }

        [JsonIgnore]
        [ObservableProperty]
        ImageSource _avatarImage;

        [ObservableProperty]
        string? _theme;
        partial void OnThemeChanged(string? value)
        {
            if (Theme == "Dark")
                Application.Current.UserAppTheme = AppTheme.Dark;
            else
                Application.Current.UserAppTheme = AppTheme.Light;
        }

        public string Initials => $"{Name?.FirstOrDefault()}{Surename?.FirstOrDefault()}".ToUpper();
    }
}
