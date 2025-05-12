using RestFullApiTest.Logic.Repositories.Interfaces;
using RestFullApiTest.Logic.Services.Interfaces;

namespace RestFullApiTest
{
    public class BookService : IBookService
    {

        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _repository.GetAllAsync();
            return books.Select(MapToDto);
        }

        //public async Task<BookDto?> GetByIdAsync(int id)
        //{
        //    var book = await _repository.GetByIdAsync(id);
        //    return book is null ? null : MapToDto(book);
        //}

        //public async Task<int> CreateAsync(CreateBookDto dto)
        //{
        //    var book = new Book
        //    {
        //        Title = dto.Title,
        //        Author = dto.Author,
        //        Genre = dto.Genre,
        //        Rating = dto.Rating
        //    };
        //    return await _repository.CreateAsync(book);
        //}

        //public async Task<bool> UpdateAsync(int id, CreateBookDto dto)
        //{
        //    var book = new Book
        //    {
        //        Id = id,
        //        Title = dto.Title,
        //        Author = dto.Author,
        //        Genre = dto.Genre,
        //        Rating = dto.Rating
        //    };
        //    return await _repository.UpdateAsync(book);
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    return await _repository.DeleteAsync(id);
        //}

        // Private mapper (optionally use AutoMapper later)
        private static BookDto MapToDto(Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Price = book.Price,
            Stock = book.Stock
        };

        public async  Task<Book?> AddBook(CreateBookDto book)
        {
            return await _repository.AddBook(book);
        }
    }
}
