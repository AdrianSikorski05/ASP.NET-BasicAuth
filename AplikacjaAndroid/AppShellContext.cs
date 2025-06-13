using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public partial class AppShellContext : ObservableObject
    {
        private Timer? _countdownTimer;
        private TimeSpan _timeLeft;

        private readonly ReadedBookStorage _readed;
        private readonly ToReadBookStorage _toRead;
        public AppShellContext(UserStorage userStorage, ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage)
        {
            UserData = userStorage;
            _readed = readedBookStorage;
            _toRead = toReadBookStorage;
            _readed.CountChanged += OnReadedCountChanged;
            _toRead.CountChanged += OnToReadCountChanged;
            UpdateUserData();
            StartCountdown();
        }

        private void OnReadedCountChanged(object? sender, EventArgs e) =>
        CountReadedBooksCount = _readed.Count;

        private void OnToReadCountChanged(object? sender, EventArgs e) =>
            CountToReadBooksCount = _toRead.Count;

        [ObservableProperty] UserStorage _userData;
        [ObservableProperty] bool _isVisibleAdminTab = false;

        [ObservableProperty] string _tokenExpiryTime = string.Empty;

        [ObservableProperty] string _countdownDisplay = "30:00";

        [ObservableProperty] int _countReadedBooksCount;
        [ObservableProperty] int _countToReadBooksCount;

        [RelayCommand]
        public void Logout()
        {
            System.Diagnostics.Debug.WriteLine("Logout command executed.");
            // Przejdź do strony logowania
            Application.Current.MainPage = App.ServiceProvider.GetRequiredService<LoginView>();
        }
     
        [RelayCommand]
        public void CloseTheFlyoutPresenter() 
        {
            Shell.Current.FlyoutIsPresented = false;
        }

        private void UpdateUserData()
        {          
            if (UserData.User.Role.ToLower() == "Admin".ToLower())
            {
                IsVisibleAdminTab = true;
            }
            else
            {
                IsVisibleAdminTab = false;
            }
        }

        public void StartCountdown()
        {
            _timeLeft = TimeSpan.FromMinutes(30);
            UpdateCountdownText();

            _countdownTimer = new Timer(OnCountdownTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void OnCountdownTick(object? state)
        {
            _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (_timeLeft <= TimeSpan.Zero)
            {
                _countdownTimer?.Dispose();
                _countdownTimer = null;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    CountdownDisplay = "00:00";                   
                    await MopupService.Instance.PopAllAsync();

                    Logout();
                    UserData.LoadData(null, new User());
                });
            }
            else
            {
                UpdateCountdownText();
            }
        }

        private void UpdateCountdownText()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CountdownDisplay = _timeLeft.ToString(@"mm\:ss");
            });
        }
    }
}
