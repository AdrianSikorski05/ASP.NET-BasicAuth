using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AplikacjaAndroid
{
    public class UsersService : IUsersService
    {
        private HttpClient _httpClient;
        private readonly UserStorage _userStorage;

        public UsersService(IHttpClientFactory factory, UserStorage userStorage)
        {
            _httpClient = factory.CreateClient("api");
            _userStorage = userStorage;
            
        }
        public async Task<ResponseResult<TokenResponse>?> Login(LoginUser loginUser)
        {
            try 
            {
                var response = await _httpClient.PostAsJsonAsync("Login/login", loginUser);

                var responseResult = await response.Content.ReadFromJsonAsync<ResponseResult<TokenResponse>>();
                if (responseResult.StatusCode == 200 && responseResult.Data != null)
                {
                    _userStorage.LoadData(responseResult, await WhoIam(responseResult.Data.Token));
                }

                return responseResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Login ERROR] {ex.Message}");
                return new ResponseResult<TokenResponse> { StatusCode = 500, Message = "Connect serwer error. " };
            }          
        }

        public async Task<User?> WhoIam(string token) 
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "Login/me");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }

                Console.WriteLine($"[WhoIam] HTTP {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhoIam ERROR] {ex.Message}");
                return null;
            }           
        }

        public async Task<ResponseResult<bool>> Register(RegisterUser registerUser)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Login/register", registerUser);

                var responseResult = await response.Content.ReadFromJsonAsync<ResponseResult<bool>>();

                return responseResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Register ERROR] {ex.Message}");
                return new ResponseResult<bool> { StatusCode = 500, Message = "Connect serwer error. ", Data = false };
            }
        }

        public async Task<bool> UpdateUser(UpdateUserDto updateUserDto) 
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
                var response = await _httpClient.PostAsJsonAsync("Users/UpdateUser", updateUserDto);

                var responseResult = await response.Content.ReadFromJsonAsync<ResponseResult<User>>();
                if (responseResult.StatusCode == 200 && responseResult.Data != null)
                {
                    _userStorage.User = responseResult.Data;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateUser ERROR] {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RefreshToken()
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Login/refresh-token", new RefreshRequestDto { RefreshToken = _userStorage.RefreshToken });
                var responseResult = await response.Content.ReadFromJsonAsync<ResponseResult<TokenResponse>>();
                if (responseResult.StatusCode == 200 && responseResult.Data != null)
                {
                    _userStorage.LoadData(responseResult, await WhoIam(responseResult.Data.Token));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RefreshToken ERROR] {ex.Message}");
                return false;
            }
        }
    }
}
