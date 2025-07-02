
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
            sb.Append(@$"SELECT c.Id [{nameof(CommentBook.id)}]
                              ,c.BookId [{nameof(CommentBook.BookId)}]
                              ,c.UserId [{nameof(CommentBook.UserId)}]
                              ,c.Author [{nameof(CommentBook.Author)}]
                              ,c.Content [{nameof(CommentBook.Content)}]
                              ,c.Rate [{nameof(CommentBook.Rate)}]
                              ,c.PublishedDate [{nameof(CommentBook.PublishedDate)}]
                              ,uc.AvatarImage [{nameof(CommentBook.AvatarImage)}]
							  ,uc.name [{nameof(CommentBook.Name)}]
							  ,uc.Surename [{nameof(CommentBook.Surename)}]
							  ,uc.AvatarColor [{nameof(CommentBook.AvatarColor)}]
                        FROM Comments c
                        inner join UserConfig uc on c.UserId = uc.UserId
                        WHERE c.BookId = @BookId");

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

        private async Task<CommentBook> GetCommentBookbyId(int id) 
        { 
            string sql = @$"SELECT c.Id [{nameof(CommentBook.id)}]
                              ,c.BookId [{nameof(CommentBook.BookId)}]
                              ,c.UserId [{nameof(CommentBook.UserId)}]
                              ,c.Author [{nameof(CommentBook.Author)}]
                              ,c.Content [{nameof(CommentBook.Content)}]
                              ,c.Rate [{nameof(CommentBook.Rate)}]
                              ,c.PublishedDate [{nameof(CommentBook.PublishedDate)}]
                              ,uc.AvatarImage [{nameof(CommentBook.AvatarImage)}]
							  ,uc.name [{nameof(CommentBook.Name)}]
							  ,uc.Surename [{nameof(CommentBook.Surename)}]
							  ,uc.AvatarColor [{nameof(CommentBook.AvatarColor)}]
                        FROM Comments c
                        inner join UserConfig uc on c.UserId = uc.UserId
                        WHERE c.Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            using var connection = dbConnectionFactory.CreateConnection();
            var comment = await connection.QueryFirstOrDefaultAsync<CommentBook>(sql, parameters);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with id {id} not found.");
            }
            return comment;
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

            var result =  await GetCommentBookbyId(id);
            return new CommentBook
            {
                id = id,
                BookId = result.BookId,
                UserId = result.UserId,
                Author = result.Author,
                Content = result.Content,
                Rate = result.Rate,
                PublishedDate = result.PublishedDate,
                AvatarImage = result.AvatarImage,
                Name = result.Name,
                Surename = result.Surename,
                AvatarColor = result.AvatarColor
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
                var commentUpdated = await GetCommentBookbyId(comment.id);
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
