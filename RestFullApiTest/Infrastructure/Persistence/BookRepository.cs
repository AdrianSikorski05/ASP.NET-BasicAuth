﻿
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
        public async Task<Book?> AddBook(CreateBookDto book)
        {
            using var connection = dbConnectionFactory.CreateConnection();
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

        /// <summary>
        /// Update book in the database.
        /// </summary>
        /// <param name="updateBook">Object of book who will updating</param>
        /// <returns></returns>
        public async Task<(Book?, int)> UpdateBook(UpdateBookDto updateBook)
        {
            using var connection = dbConnectionFactory.CreateConnection();
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
            sb.Append($" where Id = @Id");
            parameters.Add("Id", updateBook.Id);

            var result = await connection.ExecuteAsync(sb.ToString(), parameters);

            if (result > 0)
            {
                // Pobierz zaktualizowaną książkę z bazy danych
                return (await GetBookById(updateBook.Id), result);
            }
            return (null, 0);
        }

        /// <summary>
        /// Delete book from the database.
        /// </summary>
        /// <param name="deleteBookDto"></param>
        /// <returns>Return a dele</returns>
        public async Task<int> DeleteBook(DeleteBookDto deleteBookDto)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = $@"DELETE FROM Books WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", deleteBookDto.Id);

            var result = await connection.ExecuteAsync(sql, parameters);
            if (result > 0)
            {
                return result;
            }

            return result;
        }

        /// <summary>
        /// Delete book by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteBookById(int id)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"delete from books where Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var result = await connection.ExecuteAsync(sql, parameters);
            if (result > 0)
            {
                return result;
            }
            return result;
        }
        #endregion

        #region QUERIES

        /// <summary>
        /// Get all books from the database.
        /// </summary>
        /// <param name="filter">Filter object</param>
        /// <returns>Return all book valid filter.</returns>
        public async Task<PagedResult<Book>> GetAllAsync(BookFilter filter)
        {
            using var connection = dbConnectionFactory.CreateConnection();

            var sqlCount = @$"SELECT COUNT(*) FROM Books Where 1 = 1 ";

            var sb = new StringBuilder();
            sb.Append(@$"SELECT Id [{nameof(Book.Id)}],
                                Title [{nameof(Book.Title)}],
                                Author [{nameof(Book.Author)}],
                                Genre [{nameof(Book.Genre)}],
                                PublishedDate [{nameof(Book.PublishedDate)}],
                                Price [{nameof(Book.Price)}],
                                Stock [{nameof(Book.Stock)}]
                        FROM Books
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
            parameters.Add("Skip", (filter.Page - 1) * filter.PageSize);

            using var multi = await connection.QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters);

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

        /// <summary>
        /// Get book by id.
        /// </summary>
        /// <param name="id">Id of book will searching.</param>
        /// <returns>Return book about fallen id.</returns>
        public async Task<Book?> GetBookById(int id)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = $@"select Id [{nameof(Book.Id)}]
                                ,Title [{nameof(Book.Title)}]
                                ,Author [{nameof(Book.Author)}]
                                ,Genre [{nameof(Book.Genre)}]
                                ,PublishedDate [{nameof(Book.PublishedDate)}]
                                ,Price [{nameof(Book.Price)}]
                                ,Stock [{nameof(Book.Stock)}]
                        from books
                        where Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var book = await connection.QueryFirstOrDefaultAsync<Book>(sql, parameters);
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
                Stock = book.Stock
            };
        }

        #endregion
    }
}
