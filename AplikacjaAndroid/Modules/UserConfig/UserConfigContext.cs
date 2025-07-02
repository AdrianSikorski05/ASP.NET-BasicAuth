using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace AplikacjaAndroid
{
    public partial class UserConfigContext : ObservableValidator
    {
        private readonly UserStorage _userStorage;
        private readonly IUsersService _usersService;
        private readonly ConfirmeDeleteAccountView _deleteAccountPopup;
        public UserConfigContext(UserStorage userStorage, IUsersService usersService, ConfirmeDeleteAccountView confirmeDeleteAccountView)
        {
            _userStorage = userStorage; 
            _usersService = usersService;
            _deleteAccountPopup = confirmeDeleteAccountView;
            LoadData();
        }
    
        [ObservableProperty]
        User _user;

        [ObservableProperty]
        UserConfig _userConfig;
     
        [ObservableProperty]
        bool _buttonVisible = true;
     
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

        private void LoadData() 
        {
            User = _userStorage.User;
            UserConfig = _userStorage.User.UserConfig;
            if (UserConfig?.AvatarBytes != null && UserConfig.AvatarBytes.Length > 0)
            {
                ButtonVisible = false;
            }
            else
            {
                ButtonVisible = true;
            }
        }

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
                    UserConfig.AvatarImage = ImageSource.FromStream(() => new MemoryStream(UserConfig.AvatarBytes));

                    if (UserConfig.AvatarImage != null)
                        ButtonVisible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error picking image: {ex.Message}");
            }
        }


        [RelayCommand]
        public async Task SaveUserConfig() 
        {
            try
            {
                var tempImage = UserConfig.AvatarImage;
                UserConfig.AvatarImage = null;

                UpdateUserDto updateUserDto = new UpdateUserDto
                {
                    Id = User.Id,
                    Username = User.Username,
                    UserConfig = this.UserConfig,
                    Enabled = true
                };

                var result = await _usersService.UpdateUser(updateUserDto);
                if (result == true)
                {
                    UserConfig.AvatarImage = tempImage;
                    await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Check));
                }
                else
                {
                    await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Error));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user config: {ex.Message}");
            }         
        }

        [ObservableProperty]
        string _errorMessage = string.Empty;

        [ObservableProperty]
        bool _errorMessageVisibility = false;

        [RelayCommand]
        public async Task ChangePassword() 
        {
            try
            {
                var strongPasswordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");

                if (string.IsNullOrWhiteSpace(User.Password) || !strongPasswordRegex.IsMatch(User.Password))
                {
                    ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
                    ErrorMessageVisibility = true;
                    return;
                }

                if (!User.ConfirmePasswordIsTheSame())
                {
                    ErrorMessage = "Passwords do not match.";
                    ErrorMessageVisibility = true;
                    return;
                }

                ErrorMessageVisibility = false;

                UpdateUserDto updateUserDto = new UpdateUserDto
                {
                    Id = User.Id,
                    Password = User.Password
                };

                var result = await _usersService.UpdateUser(updateUserDto);
                if (result == true)
                {
                    await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Check));
                }
                else
                {
                    await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Error));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user config: {ex.Message}");
            }
        }

        private bool _isPopupOpen = false;

        [RelayCommand]
        public async Task DeleteAccount() 
        {
            if (_isPopupOpen)
                return;
            _isPopupOpen = true;

            try
            {
                UpdateUserDto updateUserDto = new UpdateUserDto
                {
                    Id = User.Id,
                    Enabled = false
                };

                var context = _deleteAccountPopup.BindingContext as ConfirmeDeleteAccountContext;
                context?.LoadContext(updateUserDto);

                await MopupService.Instance.PushAsync(_deleteAccountPopup, true);             
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user config: {ex.Message}");
            }
            _isPopupOpen = false;
        }

        [RelayCommand]
        public void DeleteAvatarImage() 
        {
            UserConfig.AvatarBytes = null!;
            UserConfig.AvatarImage = null!;
            ButtonVisible = true;
        }
    }
}
