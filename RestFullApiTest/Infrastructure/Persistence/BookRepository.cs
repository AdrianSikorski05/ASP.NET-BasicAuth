
using System.Data;
using System.Text;
using Dapper;

namespace RestFullApiTest
{
    public class BookRepository(IDbConnectionFactory dbConnectionFactory) : IBookRepository
    {

        #region COMMANDS

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async Task<Book?> AddBook(CreateBookDto book, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(nameof(Book.Title), book.Title);

                var existingBook = await connection.ExecuteScalarAsync<bool>("select exists(select * from Books where Title = @Title)", new { Title = book.Title }, transaction);
                if (existingBook)
                {
                    throw new Exception($"Book with title '{book.Title}' already exists.");
                }

                var sql = @$"INSERT INTO Books ({nameof(CreateBookDto.Title)}, {nameof(CreateBookDto.Author)}, {nameof(CreateBookDto.Genre)}, {nameof(Book.PublishedDate)}, {nameof(CreateBookDto.Price)}, {nameof(CreateBookDto.Stock)},{nameof(CreateBookDto.Image)},{nameof(CreateBookDto.Description)})
                        VALUES (@Title, @Author, @Genre, @PublishedDate, @Price, @Stock, @Image, @Description);
                        SELECT last_insert_rowid();";
                parameters.Add(nameof(Book.Author), book.Author);
                parameters.Add(nameof(Book.Genre), book.Genre);
                parameters.Add(nameof(Book.PublishedDate), DateTime.Now);
                parameters.Add(nameof(Book.Price), book.Price);
                parameters.Add(nameof(Book.Stock), book.Stock);
                parameters.Add(nameof(Book.Image), book.Image);
                parameters.Add(nameof(Book.Description), book.Description);
                var bookId = await connection.ExecuteScalarAsync<long>(sql, parameters, transaction);
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
                        Stock = book.Stock,
                        Image = book.Image,
                        Description = book.Description
                    };
                }
                return null;
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        /// <summary>
        /// Update book in the database.
        /// </summary>
        /// <param name="updateBook">Object of book who will updating</param>
        /// <returns></returns>
        public async Task<(Book?, int)> UpdateBook(UpdateBookDto updateBook, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sb = new StringBuilder();
                sb.Append("update books set Id = @Id");

                var parameters = new DynamicParameters();
                if (!string.IsNullOrWhiteSpace(updateBook.Title))
                {
                    sb.Append($", Title = @Title");
                    parameters.Add("Title", updateBook.Title);
                }
                if (!string.IsNullOrWhiteSpace(updateBook.Author))
                {
                    sb.Append($", Author = @Author");
                    parameters.Add("Author", updateBook.Author);
                }
                if (!string.IsNullOrWhiteSpace(updateBook.Genre))
                {
                    sb.Append($", Genre = @Genre");
                    parameters.Add("Genre", updateBook.Genre);
                }
                if (updateBook.Price != null)
                {
                    sb.Append($", Price = @Price");
                    parameters.Add("Price", updateBook.Price);
                }
                if (updateBook.Stock != null)
                {
                    sb.Append($", Stock = @Stock");
                    parameters.Add("Stock", updateBook.Stock);
                }

                if (updateBook.Image != null && updateBook.Image.Length > 0)
                {
                    sb.Append($", Image = @Image");
                    parameters.Add("Image", updateBook.Image);
                }

                if (!string.IsNullOrWhiteSpace(updateBook.Description))
                {
                    sb.Append($", Description = @Description");
                    parameters.Add("Description", updateBook.Description);
                }

                sb.Append($" where Id = @Id");
                parameters.Add("Id", updateBook.Id);

                var result = await connection.ExecuteAsync(sb.ToString(), parameters, transaction);

                if (result > 0)
                {
                    // Pobierz zaktualizowaną książkę z bazy danych
                    return (await GetBookById(updateBook.Id, connection, transaction), result);
                }
                return (null, result);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        /// <summary>
        /// Delete book from the database.
        /// </summary>
        /// <param name="deleteBookDto"></param>
        /// <returns>Return a dele</returns>
        public async Task<int> DeleteBook(DeleteBookDto deleteBookDto, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sql = $@"DELETE FROM Books WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", deleteBookDto.Id);

                var result = await connection.ExecuteAsync(sql, parameters, transaction);
                if (result > 0)
                {
                    return result;
                }

                return result;
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        /// <summary>
        /// Delete book by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteBookById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sql = @$"delete from books where Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id);

                var result = await connection.ExecuteAsync(sql, parameters, transaction);
                if (result > 0)
                {
                    return result;
                }
                return result;
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        public async Task<bool> UpdateBookActivityStatus(ActivityBook activityBook, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                string sql = "" ;
                var parameters = new DynamicParameters();

                if (activityBook.Status == BookStatus.AddedToToRead)
                {
                    sql = $@"INSERT INTO ActivityBook (UserId, BookId, Status) 
                            VALUES (@UserId, @BookId, ""toRead"")";                    
                }
                else if(activityBook.Status == BookStatus.AddedToReaded)
                {
                    sql = $@"INSERT INTO ActivityBook (UserId, BookId, Status) 
                            VALUES (@UserId, @BookId, ""readed"")";
                }
                else if (activityBook.Status == BookStatus.DeletedFromToRead)
                {
                    sql = $@"delete from ActivityBook
                            where UserId = @UserId and BookId = @BookId and Status = ""toRead""";
                }
                else if (activityBook.Status == BookStatus.DeletedFromReaded)
                {
                    sql = $@"delete from ActivityBook
                            where UserId = @UserId and BookId = @BookId and Status = ""readed""";
                }
                else
                {
                    return false;
                }
                
                
                parameters.Add("UserId", activityBook.UserId);
                parameters.Add("BookId", activityBook.Book.Id);
                
                var result = await connection.ExecuteAsync(sql, parameters, transaction);
                return result > 0;

            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        #endregion

        #region QUERIES

        /// <summary>
        /// Get all books from the database.
        /// </summary>
        /// <param name="filter">Filter object</param>
        /// <returns>Return all book valid filter.</returns>
        public async Task<PagedResult<Book>> GetAllAsync(BookFilter filter, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sqlCount = @$"SELECT CAST(COUNT(*) AS int) FROM Books WHERE 1=1 ";

                var sb = new StringBuilder();
                sb.Append(@$"SELECT 
                                b.Id [{nameof(Book.Id)}],
                                b.Title [{nameof(Book.Title)}],
                                b.Author [{nameof(Book.Author)}],
                                b.Genre [{nameof(Book.Genre)}],
                                b.PublishedDate [{nameof(Book.PublishedDate)}],
                                b.Price [{nameof(Book.Price)}],
                                b.Stock [{nameof(Book.Stock)}],
                                b.Image [{nameof(Book.Image)}],
                                b.Description [{nameof(Book.Description)}],
                                ROUND(SUM(c.Rate) * 1.0 / NULLIF(COUNT(c.Rate), 0), 1) [{nameof(Book.AverageRating)}]
                            FROM Books b
                            LEFT JOIN Comments c ON b.Id = c.BookId
                        Where 1 = 1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(filter.Author))
                {
                    sb.Append($" and {nameof(BookFilter.Author)} = @Author ");
                    parameters.Add("Author", filter.Author);
                }

                if (!string.IsNullOrWhiteSpace(filter.Genre))
                {
                    sb.Append($" and {nameof(BookFilter.Genre)} = @Genre ");
                    parameters.Add($"Genre", filter.Genre);
                }
                if (!string.IsNullOrWhiteSpace(filter.Title))
                {
                    sb.Append($" and {nameof(BookFilter.Title)} = @Title ");
                    parameters.Add($"Title", filter.Title);
                }

                sb.Append(" group by b.Id, b.Title, b.Author, b.Genre, b.PublishedDate, b.Price, b.Stock, b.Image, b.Description ");

                var sortColumn = filter.SortBy.ToLower() switch
                {
                    "title" => "Title",
                    "author" => "Author",
                    "genre" => "Genre",
                    "price" => "Price",
                    _ => "Title"
                };

                var sortDir = filter.SortDir.ToLower() == "desc" ? "DESC" : "ASC";

                sb.Append($" ORDER BY {sortColumn} {sortDir} ");


                sb.Append(" limit @PageSize offset @Skip ");

                parameters.Add("PageSize", filter.PageSize);
                var skip = (filter.Page - 1) * filter.PageSize;
                parameters.Add("Skip", skip);

                using var multi = await connection.QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters, transaction);

                var totalCount = await multi.ReadFirstAsync<int>();
                var books = await multi.ReadAsync<Book>();

                return new PagedResult<Book>
                {
                    Items = books,
                    TotalItems = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        /// <summary>
        /// Get book by id.
        /// </summary>
        /// <param name="id">Id of book will searching.</param>
        /// <returns>Return book about fallen id.</returns>
        public async Task<Book?> GetBookById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sql = $@"select Id [{nameof(Book.Id)}]
                                ,Title [{nameof(Book.Title)}]
                                ,Author [{nameof(Book.Author)}]
                                ,Genre [{nameof(Book.Genre)}]
                                ,PublishedDate [{nameof(Book.PublishedDate)}]
                                ,Price [{nameof(Book.Price)}]
                                ,Stock [{nameof(Book.Stock)}]
                                ,Image [{nameof(Book.Image)}]
                                ,Description [{nameof(Book.Description)}]
                        from books
                        where Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id);

                var book = await connection.QueryFirstOrDefaultAsync<Book>(sql, parameters, transaction);
                if (book == null)
                {
                    throw new Exception("Dont found a book.");
                }
                return new Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    PublishedDate = book.PublishedDate,
                    Price = book.Price,
                    Stock = book.Stock,
                    Image = book.Image,
                    Description = book.Description
                };
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        public async Task<List<Book?>> GetBookWithActivityStatusByUser(DataStatusBookWithUserIdDto data, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
                var sql = $@"select b.Id [{nameof(Book.Id)}]
                                    ,b.Title [{nameof(Book.Title)}]
                                    ,b.Genre [{nameof(Book.Genre)}]
                                    ,b.Author [{nameof(Book.Author)}]
                                    ,b.PublishedDate [{nameof(Book.PublishedDate)}]
                                    ,b.Price [{nameof(Book.Price)}]
                                    ,b.Stock [{nameof(Book.Stock)}]
                                    ,b.Image [{nameof(Book.Image)}]
                                    ,b.Description [{nameof(Book.Description)}]
                            from ActivityBook ab 
                            inner join books b on b.Id = ab.BookId
                            where ab.UserId = @UserId and ab.Status = @Status";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", data.UserId);
                parameters.Add("Status", data.StatusBook);

                var books = await connection.QueryAsync<Book>(sql, parameters, transaction);
                return books.ToList();
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        #endregion
    }
}
