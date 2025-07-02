namespace AplikacjaAndroid
{
    public interface IUsersService
    {
        Task<ResponseResult<TokenResponse>?> Login(LoginUser loginUser);
        Task<ResponseResult<bool>> Register(RegisterUser registerUser);
        Task<bool> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> RefreshToken();
    }
}
