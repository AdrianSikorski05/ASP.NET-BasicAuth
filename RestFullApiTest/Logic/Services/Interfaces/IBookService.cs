namespace RestFullApiTest.Logic.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<Book?> AddBook(CreateBookDto book);
        //Task<BookDto?> GetByIdAsync(int id);
        //Task<int> CreateAsync(CreateBookDto book);
        //Task<bool> UpdateAsync(int id, CreateBookDto book);
        //Task<bool> DeleteAsync(int id);
    }
}
