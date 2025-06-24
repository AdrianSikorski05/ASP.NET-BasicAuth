using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public interface IUsersService
    {
        Task<ResponseResult<TokenResponse>?> Login(LoginUser loginUser);

        Task<ResponseResult<bool>> Register(RegisterUser registerUser);
    }
}
