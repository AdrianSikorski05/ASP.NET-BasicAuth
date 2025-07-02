using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace AplikacjaAndroid
{
    public partial class ConfirmeDeleteAccountContext : ObservableObject
    {
        private readonly IUsersService _usersService;
        private readonly UserStorage _userStorage;
        private UpdateUserDto _user = new();

        public ConfirmeDeleteAccountContext(UserStorage userStorage, IUsersService usersService)
        {
            _userStorage = userStorage;
            _usersService = usersService;
        }

        [RelayCommand]
        public async Task DeleteAccount() 
        {

            var result = await _usersService.UpdateUser(_user);
            if (result == true)
            {
                await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Check));
                await Task.Delay(1500);
                //// Log out user after deletion
                await MopupService.Instance.PopAllAsync();
                _userStorage.Logout();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = App.ServiceProvider.GetRequiredService<LoginView>();
                });
            }
            else
            {
                await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Error));
            }        
        }


        [RelayCommand]
        public async Task Close()
        {
            await MopupService.Instance.PopAllAsync();
        }

        public void LoadContext(UpdateUserDto user)
        {
            _user = user;
        }
    }
}
