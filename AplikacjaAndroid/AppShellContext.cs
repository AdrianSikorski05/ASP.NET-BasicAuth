using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Diagnostics;

namespace AplikacjaAndroid
{
    public partial class AppShellContext : ObservableObject
    {
        private Timer? _countdownTimer;
        private TimeSpan _timeLeft;

        private readonly ReadedBookStorage _readed;
        private readonly ToReadBookStorage _toRead;
        private readonly NavigationService _navigationService;
        private readonly IUsersService _usersService;

        public AppShellContext(UserStorage userStorage, ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage, NavigationService navigationService,IUsersService usersService)
        {
            UserData = userStorage;
            _readed = readedBookStorage;
            _toRead = toReadBookStorage;
            _readed.CountChanged += OnReadedCountChanged;
            _toRead.CountChanged += OnToReadCountChanged;
            UpdateUserData();
            StartCountdown();
            CountReadedBooksCount = _readed.Count;
            CountToReadBooksCount = _toRead.Count;
            _navigationService = navigationService;          
            _usersService = usersService;
        }

        private void OnReadedCountChanged(object? sender, EventArgs e) =>
        CountReadedBooksCount = _readed.Count;

        private void OnToReadCountChanged(object? sender, EventArgs e) =>
            CountToReadBooksCount = _toRead.Count;

        [ObservableProperty]
        string _titleToRead = "Book to read (0)";

        [ObservableProperty]
        string _titleReaded = "Readed book (0)";

        partial void OnCountToReadBooksCountChanged(int value)
        {
            TitleToRead = $"Book to read ({value})";
        }

        partial void OnCountReadedBooksCountChanged(int value)
        {
            TitleReaded = $"Readed book ({value})";
        }

        [ObservableProperty] UserStorage _userData;
        [ObservableProperty] bool _isVisibleAdminTab = false;

        [ObservableProperty] string _tokenExpiryTime = string.Empty;

        [ObservableProperty] string _countdownDisplay = "30:00";

        [ObservableProperty] int _countReadedBooksCount;
        [ObservableProperty] int _countToReadBooksCount;
        [ObservableProperty] bool _isBusy;
        [RelayCommand]
        public void Logout()
        {
            UserData.Logout();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = App.ServiceProvider.GetRequiredService<LoginView>();
            });
        }
     
        [RelayCommand]
        public void CloseTheFlyoutPresenter() 
        {
            Shell.Current.FlyoutIsPresented = false;
        }

        private void UpdateUserData()
        {
            if (!string.IsNullOrWhiteSpace(UserData?.User?.Role) && UserData.User.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
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
            _timeLeft = TimeSpan.FromMinutes(60);
            UpdateCountdownText();

            _countdownTimer = new Timer(OnCountdownTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private bool _refreshPopupShown = false;
        private TaskCompletionSource<bool> _sessionResponseTcs;
        private void OnCountdownTick(object? state)
        {
            _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (_timeLeft <= TimeSpan.FromMinutes(2) && !_refreshPopupShown)
            {
                _refreshPopupShown = true;
                _sessionResponseTcs = new TaskCompletionSource<bool>();

                
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var context = new RefreshTokenPopupContext();
                    context.SetCompletionSource(_sessionResponseTcs);
                    var popup = new RefreshTokenPopupView(context);

                    await MopupService.Instance.PushAsync(popup);

                   //oczekiwanie na wynik
                   var result = await _sessionResponseTcs.Task;
                    if (result)
                    {
                        IsBusy = true;
                        var res = await _usersService.RefreshToken();
                        if (res == true)
                        {
                            _timeLeft = TimeSpan.FromSeconds(60);
                            _refreshPopupShown = false;
                        }
                        IsBusy = false;
                    }
                });
            }


            if (_timeLeft <= TimeSpan.Zero)
            {
                _countdownTimer?.Dispose();
                _countdownTimer = null;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        CountdownDisplay = "00:00";
                        await MopupService.Instance.PopAllAsync();

                        Logout();
                        UserData.LoadData(null, new User());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Countdown Error] {ex}");
                    }
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

        private bool _isNavigating = false;

        [ObservableProperty]
        bool _navigation = false;

        [RelayCommand]
        public async Task OpenUserConfig() 
        {
            Navigation = false;

            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                await _navigationService.NavigateToAsync(nameof(UserConfigView));
            }
            finally
            {
                _isNavigating = false;
            }
        }   
    }
}
