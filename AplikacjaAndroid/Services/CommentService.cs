using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace AplikacjaAndroid
{
    public class CommentService : ICommentService
    {
        private HttpClient _httpClient;
        private readonly UserStorage _userStorage;

        public CommentService(IHttpClientFactory factory, UserStorage userStorage)
        {
            _httpClient = factory.CreateClient("api");
            _userStorage = userStorage;
        }

        public async Task<ResponseResult<CommentBook?>> AddCommentAsync(CreateCommentBookDto comment)
        {
            try
            {
                var url = $"Comment/addComment";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
                var response = await _httpClient.PostAsJsonAsync(url, comment);

                return await response.Content.ReadFromJsonAsync<ResponseResult<CommentBook>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddCommentAsync ERROR] {ex.Message}");
                return new ResponseResult<CommentBook?> { StatusCode = 500, Message = "Connect serwer error. " };
            }         
        }

        public async Task<ResponseResult<PagedResult<CommentBook>>> GetComments(CommentFilter commentFilter)
        {
            try
            {
                var query = ToQueryString(commentFilter);
                var url = $"Comment/comments?{query}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
                var response = await _httpClient.GetAsync(url);

                return await response.Content.ReadFromJsonAsync<ResponseResult<PagedResult<CommentBook>>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetComments ERROR] {ex.Message}");
                return new ResponseResult<PagedResult<CommentBook>> { StatusCode = 500, Message = "Connect serwer error. " };
            }           
        }

        public async Task<ResponseResult<CommentBook?>> UpdateCommentAsync(UpdateCommentBookDto comment)
        {
            try
            {
                var url = $"Comment/updateComment";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
                var response = await _httpClient.PostAsJsonAsync(url, comment);

                return await response.Content.ReadFromJsonAsync<ResponseResult<CommentBook>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateCommentAsync ERROR] {ex.Message}");
                return new ResponseResult<CommentBook?> { StatusCode = 500, Message = "Connect serwer error. " };
            }           
        }

        public async Task<ResponseResult<bool>> DeleteCommentAsync(int id)
        {
            try
            {
                var url = $"Comment/deleteComment/{id}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userStorage.Token);
                var response = await _httpClient.DeleteAsync(url);
                return await response.Content.ReadFromJsonAsync<ResponseResult<bool>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteCommentAsync ERROR] {ex.Message}");
                return new ResponseResult<bool> { StatusCode = 500, Message = "Connect serwer error. " };
            }
        }

        private string ToQueryString(CommentFilter filter)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filter.Page > 0)
                query["Page"] = filter.Page.ToString();

            if (filter.PageSize > 0)
                query["PageSize"] = filter.PageSize.ToString();

            if (filter.BookId > 0)
                query["BookId"] = filter.BookId.ToString();
          
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                query["SortBy"] = filter.SortBy;

            if (!string.IsNullOrWhiteSpace(filter.SortDir))
                query["SortDir"] = filter.SortDir;

            return query.ToString();
        }
    }
}
