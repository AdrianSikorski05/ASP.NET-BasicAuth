

using System.Data;

namespace RestFullApiTest
{
    public interface IBookRepository
    {
        Task<PagedResult<Book>> GetAllAsync(BookFilter filter, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<Book?> GetBookById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<Book?> AddBook(CreateBookDto newBook, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<(Book?,int)> UpdateBook(UpdateBookDto updateBook, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int> DeleteBook(DeleteBookDto deleteBook, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int> DeleteBookById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<List<Book?>> GetBookWithActivityStatusByUser(DataStatusBookWithUserIdDto data, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<bool> UpdateBookActivityStatus(ActivityBook activityBook, IDbConnection? connection = null, IDbTransaction? transaction = null);
    }
}
