using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Diagnostics;

namespace AplikacjaAndroid;

public partial class LoginContext(IUsersService usersService, ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage) : ObservableObject
{
    [ObservableProperty]
    private LoginUser _loginUser = new();

    [ObservableProperty]
    private RegisterUser _registerUser = new();

    [ObservableProperty]
    private bool _labelVisibleLogin = false;

    [ObservableProperty]
    private string _errorMessageLogin = string.Empty;

    [ObservableProperty]
    private bool _loginRegister = true;

    [ObservableProperty]
    private bool _labelVisibleRegister = false;

    [ObservableProperty]
    private string _errorMessageRegister = string.Empty;

    [RelayCommand]
    public async Task Login()
    {
        try
        {
            if (!LoginUser.IsValid())
            {
                ErrorMessageLogin = "Field username and password is required.";
                LabelVisibleLogin = true;
                return;
            }
            var response = await usersService.Login(LoginUser);
            if (response.StatusCode == 200)
            {
                // zabezpiecz dane
                if (response.Data == null)
                {
                    ErrorMessageLogin = "Login failed. Missing user data.";
                    LabelVisibleLogin = true;
                    return;
                }

                // PRZED zmian¹ MainPage – ustaw dane i zainicjalizuj
                await readedBookStorage.LoadData();
                await toReadBookStorage.LoadData();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                // NA KOÑCU — na g³ównym w¹tku zmieñ stronê
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = App.ServiceProvider.GetRequiredService<AppShell>();
                });
            }
            else if (response.StatusCode == 401 && response.Message == "Invalid username or password")
            {
                ErrorMessageLogin = response.Message;
                LabelVisibleLogin = true;
            }
            else if (response.StatusCode == 401 && response.Message == "Account is disabled.")
            {
                ErrorMessageLogin = response.Message;
                LabelVisibleLogin = true;
            }
            else
            {
                ErrorMessageLogin = response.Message;
                LabelVisibleLogin = true;
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }
    }

    [RelayCommand]
    public async Task Register()
    {
        try
        {
            if (!RegisterUser.IsValid())
            {
                LabelVisibleRegister = true;
                ErrorMessageRegister = RegisterUser.GetErrors().FirstOrDefault()?.ErrorMessage ?? "Validation error.";
                return;
            }
            if (!RegisterUser.ConfirmePasswordIsTheSame())
            {
                LabelVisibleRegister = true;
                ErrorMessageRegister = "Passwords aren't the same.";
                return;
            }

            var response = await usersService.Register(RegisterUser);
            if (response != null && response.StatusCode == 200 && response.Data == true)
            {
                await MopupService.Instance.PushAsync(new SuccessPopupView(AnimationType.Check));
                RegisterUser = new();
                LabelVisibleRegister = false;
            }
            else
            {
                LabelVisibleRegister = true;
                ErrorMessageRegister = response.Message;
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }
    }

    [RelayCommand]
    public void ChangeMode()
    {
        LoginRegister = !LoginRegister;
    }
}