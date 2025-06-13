using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public class UserStorage
    {
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public void LoadData(ResponseResult<TokenResponse>? response, User user) 
        {
            Token = response?.Data?.Token;
            RefreshToken = response?.Data?.RefreshToken;
            User = user;
        }
    }
}
