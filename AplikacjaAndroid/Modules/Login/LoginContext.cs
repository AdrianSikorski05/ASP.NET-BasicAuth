using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AplikacjaAndroid;

public partial class LoginContext(IUsersService usersService, ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage) : ObservableObject
{
    [ObservableProperty]
    private LoginUser _loginUser = new();

    [ObservableProperty]
    private bool _labelVisible = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;


    [RelayCommand]
	public async Task Login()
	{
        try
        {
            if (!LoginUser.IsValid())
            {
                ErrorMessage = "Field username and password is required.";
                LabelVisible = true;
                return;
            }

            var response = await usersService.Login(LoginUser);
            if (response.StatusCode == 200)
            {
                // Tworzymy nowy Shell i podmieniamy stronê g³ówn¹ aplikacji
                await readedBookStorage.LoadData();
                await toReadBookStorage.LoadData();
                Application.Current.MainPage = App.ServiceProvider.GetRequiredService<AppShell>();
            }
            else if(response.StatusCode == 401 && response.Message == "Invalid username or password")
            {
                ErrorMessage = response.Message;
                LabelVisible = true;
            }
            else 
            {
                ErrorMessage = response.Message;
                LabelVisible = true;
            }           
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }       
    }	
}