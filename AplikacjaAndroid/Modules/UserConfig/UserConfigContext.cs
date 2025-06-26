
using Android.Hardware.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;


namespace AplikacjaAndroid
{
    public partial class UserConfigContext : ObservableValidator
    {
        public UserConfigContext()
        {
            _userConfig.PropertyChanged += ViewModelOnPropertyChanged;
        }
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(nameof(Initials));
        }

        [ObservableProperty]
        UserConfig _userConfig = new UserConfig();


        [ObservableProperty]
        ImageSource _avatarImage;
     
        [ObservableProperty]
        bool _buttonVisible = true;

        public string Initials => $"{UserConfig.Name?.FirstOrDefault()}{UserConfig.Surename?.FirstOrDefault()}".ToUpper();

        public ObservableCollection<Color> AvailableColors { get; } = new()
        {
            Color.FromArgb("#A3D5FF"), // Baby Blue
            Color.FromArgb("#FFC1CC"), // Baby Pink
            Color.FromArgb("#C8A2C8"), // Lilac
            Color.FromArgb("#A8E6CF"), // Soft Mint
            Color.FromArgb("#FFFACD"), // Lemon Chiffon
            Color.FromArgb("#B0E0E6"), // Powder Blue
            Color.FromArgb("#E6E6FA"), // Light Lavender
            Color.FromArgb("#FFDAB9"), // Light Peach
            Color.FromArgb("#D1C4E9")  // Baby Purple
        };
        public ObservableCollection<string> Themes { get; } = new()
        {
            "Light",
            "Dark"
        };

        [RelayCommand]
        public async Task ChangeAvatarAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select an avatar",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Z memoryStream robimy wszystko
                    UserConfig.AvatarBytes = memoryStream.ToArray();

                    // Uwaga: musisz zrobić osobny MemoryStream, bo FromStream zużyje go:
                    AvatarImage = ImageSource.FromStream(() => new MemoryStream(UserConfig.AvatarBytes));

                    if (AvatarImage != null)
                        ButtonVisible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error picking image: {ex.Message}");
            }
        }

        [RelayCommand]
        public void DeleteAvatarImage() 
        {
            UserConfig.AvatarBytes = null!;
            AvatarImage = null!;
            ButtonVisible = true;
        }
    }
}
