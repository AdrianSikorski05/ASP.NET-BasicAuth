using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AplikacjaAndroid
{
    public class UsersService: IUsersService
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
            var response = await _httpClient.PostAsJsonAsync("Login/login", loginUser);

            var responseResult = await response.Content.ReadFromJsonAsync<ResponseResult<TokenResponse>>();

            if (responseResult.StatusCode == 200 && responseResult.Data != null)
            {
                _userStorage.LoadData(responseResult, await WhoIam(responseResult.Data.Token));
            }

            return responseResult;
        }

        public async Task<User?> WhoIam(string token) 
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("Login/me");
            var user = await response.Content.ReadFromJsonAsync<User>();
            return user;
        }
    }
}
