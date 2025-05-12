namespace RestFullApiTest.Logic.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> AddBook(CreateBookDto book);

    }
}
