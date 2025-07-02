using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public partial class UserStorage : ObservableValidator
    {
        [ObservableProperty]
        User? _user;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public void LoadData(ResponseResult<TokenResponse>? response, User user) 
        {
            Token = response?.Data?.Token;
            RefreshToken = response?.Data?.RefreshToken;
            User = user;
        }

        public void Logout()
        {
            User = null;
            Token = null;
            RefreshToken = null;
        }
    }
}
