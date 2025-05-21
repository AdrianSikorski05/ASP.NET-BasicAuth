

namespace RestFullApiTest
{
    public interface IBookRepository
    {
        Task<PagedResult<Book>> GetAllAsync(BookFilter filter);
        Task<Book?> GetBookById(int id);
        Task<Book?> AddBook(CreateBookDto newBook);
        Task<(Book?,int)> UpdateBook(UpdateBookDto updateBook);
        Task<int> DeleteBook(DeleteBookDto deleteBook);
        Task<int> DeleteBookById(int id);

    }
}
