
using Dapper;
using RestFullApiTest.Logic.Repositories.Interfaces;

namespace RestFullApiTest
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public BookRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<Book?> AddBook(CreateBookDto book)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"INSERT INTO Books ({nameof(Book.Title)}, {nameof(Book.Author)}, {nameof(Book.Genre)}, {nameof(Book.PublishedDate)}, {nameof(Book.Price)}, {nameof(Book.Stock)})
                        VALUES (@Title, @Author, @Genre, @PublishedDate, @Price, @Stock);
                        SELECT last_insert_rowid();";
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Book.Title), book.Title);
            parameters.Add(nameof(Book.Author), book.Author);
            parameters.Add(nameof(Book.Genre), book.Genre);
            parameters.Add(nameof(Book.PublishedDate), DateTime.Now);
            parameters.Add(nameof(Book.Price), book.Price);
            parameters.Add(nameof(Book.Stock), book.Stock);
            var bookId = await connection.ExecuteScalarAsync<long>(sql, parameters);
            if (bookId > 0)
            {
                return new Book
                {
                    Id = (int)bookId,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    PublishedDate = DateTime.Now,
                    Price = book.Price,
                    Stock = book.Stock
                };
            }
            return null;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"SELECT Id [{nameof(Book.Id)}],
                                Title [{nameof(Book.Title)}],
                                Author [{nameof(Book.Author)}],
                                Genre [{nameof(Book.Genre)}],
                                PublishedDate [{nameof(Book.PublishedDate)}],
                                Price [{nameof(Book.Price)}],
                                Stock [{nameof(Book.Stock)}]
                        FROM Books";
            return await connection.QueryAsync<Book>(sql);

        }
    }
}
