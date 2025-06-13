namespace AplikacjaAndroid
{
    public interface IBooksService
    {
        Task<ResponseResult<PagedResult<Book>>?> GetAllBooks(BookFilter bookFilter);
    }
}
