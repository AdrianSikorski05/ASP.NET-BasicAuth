using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace RestFullApiTest.IntegrationsTests
{
    public class BookRepositoryTests
    {
        [Theory]
        [InlineData("test2", "test2", "test2", 21.90, 29)]
        [InlineData("test3", "test3", "test31", 22.90, 39)]
        [InlineData("test4", "test4", "test1", 20.90, 99)]
        public async void AddBook_Should_InsertNewBook_When_DataIsValidAndBookDoesntExists(string title, string author, string genre, decimal price, int stock)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            var bookRepository = new BookRepository(connection);
            var book = new CreateBookDto(title, author, genre, price, stock);

            try
            {
                // Act
                var newBook = await bookRepository.AddBook(book, dbConnection, transaction);

                // Assert
                newBook.Should().NotBeNull();
                newBook.Id.Should().BeGreaterThan(0);
                newBook.Title.Should().Be(title);
                newBook.Author.Should().Be(author);
                newBook.Genre.Should().Be(genre);
                newBook.Price.Should().Be(price);
                newBook.Stock.Should().Be(stock);
                // Verify that the book was added to the database
                var addedBook = await bookRepository.GetBookById(newBook.Id, dbConnection, transaction);
                addedBook.Should().NotBeNull();
                addedBook.Id.Should().Be(newBook.Id);

            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Fact]
        public async void AddBook_Should_ThrowException_When_BookAlreadyExistsWithTheSameTitle()
        {
            // Arange
            var connection = new SqliteConnectionFactory();
            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            var bookRepository = new BookRepository(connection);

            var book = new CreateBookDto("test1", "test1", "test1", 21.90m, 29);

            // Act

            Func<Task> act = async () =>
            {
                await bookRepository.AddBook(book, dbConnection, transaction);
                await bookRepository.AddBook(book, dbConnection, transaction); // Attempt to add the same book again
            };

            // Assert

            await act.Should().ThrowAsync<Exception>().WithMessage($"Book with title '{book.Title}' already exists.");
        }

        public static IEnumerable<object[]> UpdateBookData =>
        new List<object[]>
        {
            new object[] { 22, "A", "B", "C", 20.10m, 15 },
            new object[] { 22, null, "B", "C", 20.10m, 15 },
            new object[] { 22, "A", null, "C", 20.10m, 15 },
            new object[] { 22, "A", "B", null, 20.10m, 15 },
            new object[] { 22, "A", "B", "C", null, 15 },
            new object[] { 22, "A", "B", "C", 20.10m, null },
            new object[] { 22, "A", null, null, null, null },
        };

        [Theory]
        [MemberData(nameof(UpdateBookData))]
        public async void UpdateBook_Should_UpdatePropertyBook_When_BookExistsAndDataIsValid(int id, string? title, string? author, string? genre, decimal? price, int? stock)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            var bookRepository = new BookRepository(connection);
            var book = new UpdateBookDto(id, title, author, genre, price, stock);
            try
            {
                // Act
                var oldDataBook = await bookRepository.GetBookById(id, dbConnection, transaction);
                var result = await bookRepository.UpdateBook(book, dbConnection, transaction);

                // Assert
                result.Item1.Should().NotBeNull();
                result.Item2.Should().Be(1);
                result.Item1.Id.Should().Be(id);

                // Verify that the book was updated in the database
                if (!string.IsNullOrEmpty(title))
                    result.Item1.Title.Should().Be(title);
                else
                    result.Item1.Title.Should().Be(oldDataBook.Title);

                if (!string.IsNullOrEmpty(author))
                    result.Item1.Author.Should().Be(author);
                else
                    result.Item1.Author.Should().Be(oldDataBook.Author);

                if (!string.IsNullOrEmpty(genre))
                    result.Item1.Genre.Should().Be(genre);
                else
                    result.Item1.Genre.Should().Be(oldDataBook.Genre);

                if (price.HasValue)
                    result.Item1.Price.Should().Be(price.Value);
                else
                    result.Item1.Price.Should().Be(oldDataBook.Price);

                if (stock.HasValue)
                    result.Item1.Stock.Should().Be(stock.Value);
                else
                    result.Item1.Stock.Should().Be(oldDataBook.Stock);

            }
            finally
            {
                transaction.Rollback();
            }
        }
    }
}
