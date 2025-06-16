using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace AplikacjaAndroid
{
    public class BooksService : IBooksService
    {
        private HttpClient _httpClient;
        private readonly UserStorage _userStorage;

        public BooksService(IHttpClientFactory factory, UserStorage userStorage)
        {
            _httpClient = factory.CreateClient("api");
            _userStorage = userStorage;
        }

        public async Task<ResponseResult<PagedResult<Book>>?> GetAllBooks(BookFilter bookFilter)
        {
            var query = ToQueryString(bookFilter);
            var url = $"Books/books?{query}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
            var response = await _httpClient.GetAsync(url);

            
            return await response.Content.ReadFromJsonAsync<ResponseResult<PagedResult<Book>>>();
        }

        public async Task<ResponseResult<List<Book>>?> GetBookWithActivityStatusByUser(DataStatusBookWithUserIdDto data)
        {
            var url = $"Books/getBookWithActivityStatusByUser";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
            var response = await _httpClient.PostAsJsonAsync(url, data);
            
            return await response.Content.ReadFromJsonAsync<ResponseResult<List<Book>>>();
            
        }

        public async Task<bool> UpdateBookActivityStatus(Book book, BookStatus status = BookStatus.Default)
        {
            var url = $"Books/updateBookActivityStatus";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
            var response = await _httpClient.PostAsJsonAsync(url, new ActivityBook {UserId = _userStorage.User.Id,Book = book,Status = status });
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            return false;
        }


        private string ToQueryString(BookFilter filter)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filter.Page > 0)
                query["Page"] = filter.Page.ToString();

            if (filter.PageSize > 0)
                query["PageSize"] = filter.PageSize.ToString();

            if (!string.IsNullOrWhiteSpace(filter.Author))
                query["Author"] = filter.Author;

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query["Title"] = filter.Title;

            if (!string.IsNullOrWhiteSpace(filter.Genre))
                query["Genre"] = filter.Genre;

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                query["SortBy"] = filter.SortBy;

            if (!string.IsNullOrWhiteSpace(filter.SortDir))
                query["SortDir"] = filter.SortDir;

            return query.ToString();
        }
    }
}
