using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplikacjaAndroid
{
    public partial class UserConfig : ObservableValidator
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ObservableProperty]
        string? _name = "New";

        [ObservableProperty]
        string? _surename = "New";

        [EmailAddress(ErrorMessage = "Have to be email.")]
        [ObservableProperty] 
        string? _email;

        [Phone(ErrorMessage = "Have to be phone number")]
        [ObservableProperty]
        string? _phoneNumber;

        [ObservableProperty]
        Color _selectedColor = Color.FromArgb("#A3D5FF");

        [ObservableProperty]
        byte[] _avatarBytes;

        [ObservableProperty]
        string _theme = "Light";
    }
}
