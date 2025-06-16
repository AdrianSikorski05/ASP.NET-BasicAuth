namespace AplikacjaAndroid
{
    public interface IBooksService
    {
        Task<ResponseResult<PagedResult<Book>>?> GetAllBooks(BookFilter bookFilter);
        Task<ResponseResult<List<Book>>?> GetBookWithActivityStatusByUser(DataStatusBookWithUserIdDto data);
        Task<bool> UpdateBookActivityStatus(Book book, BookStatus status = BookStatus.Default);
    }
}
