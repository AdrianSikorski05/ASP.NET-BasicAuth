
using Dapper;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RestFullApiTest
{
    public class CommentRepository(IDbConnectionFactory dbConnectionFactory) : ICommentRepository
    {
        #region Queries

        public async Task<PagedResult<CommentBook>> GetCommentsAsync(CommentFilter commentFilter)
        {

            var sqlCount = @"SELECT COUNT(*) FROM Comments WHERE BookId = @BookId";

            var sb = new StringBuilder();
            sb.Append(@$"SELECT Id [{nameof(CommentBook.id)}]
                              ,BookId [{nameof(CommentBook.BookId)}]
                              ,UserId [{nameof(CommentBook.UserId)}]
                              ,Author [{nameof(CommentBook.Author)}]
                              ,Content [{nameof(CommentBook.Content)}]
                              ,Rate [{nameof(CommentBook.Rate)}]
                              ,PublishedDate [{nameof(CommentBook.PublishedDate)}]
                        FROM Comments
                        WHERE BookId = @BookId");

            var parameters = new DynamicParameters();
            parameters.Add("BookId", commentFilter.BookId);
            var sortDir = commentFilter.SortDir.ToLower() == "desc" ? "DESC" : "ASC";

            sb.Append($" ORDER BY {commentFilter.SortBy} {sortDir} ");

            sb.Append(" limit @PageSize offset @Skip ");
            parameters.Add("PageSize", commentFilter.PageSize);
            parameters.Add("Skip", (commentFilter.Page - 1) * commentFilter.PageSize);

            using var multiResults = await dbConnectionFactory.CreateConnection().QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters);

            var totalCount = await multiResults.ReadFirstAsync<int>();
            var comments = await multiResults.ReadAsync<CommentBook>();

            return new PagedResult<CommentBook>
            {
                Items = comments,
                TotalItems = totalCount,
                Page = commentFilter.Page,
                PageSize = commentFilter.PageSize
            };
        }
        #endregion


        #region Commands
        public async Task<CommentBook> AddCommentAsync(CreateCommentBookDto comment)
        {
            var sql = @"INSERT INTO Comments (BookId, UserId, Author, Content, Rate, PublishedDate) 
                        VALUES (@BookId, @UserId, @Author, @Content, @Rate, @PublishedDate);
                        SELECT last_insert_rowid();";

            var parameters = new DynamicParameters();
            parameters.Add("BookId", comment.BookId);
            parameters.Add("UserId", comment.UserId);
            parameters.Add("Author", comment.Author);
            parameters.Add("Content", comment.Content);
            parameters.Add("Rate", comment.Rate);
            parameters.Add("PublishedDate", comment.PublishedDate);

            using var connection = dbConnectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(sql, parameters);

            return new CommentBook
            {
                id = id,
                BookId = comment.BookId,
                UserId = comment.UserId,
                Author = comment.Author,
                Content = comment.Content,
                Rate = comment.Rate,
                PublishedDate = comment.PublishedDate
            };

        }

        public async Task<CommentBook> UpdateCommentAsync(UpdateCommentBookDto comment)
        {

            var sb = new StringBuilder();
            var parameters = new DynamicParameters();

            sb.Append("UPDATE Comments SET ");
            var updates = new List<string>();

            // zawsze aktualizujemy autora (opcjonalnie możesz to pominąć)
            updates.Add("Author = @Author");
            parameters.Add("Author", comment.Author);

            if (!string.IsNullOrWhiteSpace(comment.Content))
            {
                updates.Add("Content = @Content");
                parameters.Add("Content", comment.Content);
            }

            if (comment.Rate.HasValue)
            {
                updates.Add("Rate = @Rate");
                parameters.Add("Rate", comment.Rate);
            }

            sb.Append(string.Join(", ", updates));
            sb.Append(" WHERE Id = @Id");
            parameters.Add("Id", comment.id);

            var result = await dbConnectionFactory.CreateConnection().ExecuteAsync(sb.ToString(), parameters);

            if (result > 0)
            {
                var commentUpdated = await dbConnectionFactory.CreateConnection().QueryFirstOrDefaultAsync<CommentBook>(
                    "SELECT * FROM Comments WHERE Id = @Id", new { Id = comment.id });
                return commentUpdated;
            }

            return null;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            string query = "DELETE FROM Comments WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            using var connection = dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(query, parameters);
            if (result > 0)
            {
                return true;

            }

            return false;
        }
        #endregion
    }
}
