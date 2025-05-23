
using System.Text;
using Dapper;

namespace RestFullApiTest
{
    public class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
    {
        #region COMMANDS
        /// <summary>
        /// Add new user to the database.
        /// </summary>
        /// <param name="user">New user</param>
        /// <returns>Return added user</returns>
        public async Task<User?> AddUser(CreateUserDto user)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"INSERT INTO Users (Username, Password)
                        VALUES (@Username, @Password);
                        SELECT last_insert_rowid();";

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var parameters = new DynamicParameters();
            parameters.Add("Username", user.Username);
            parameters.Add("Password", hashedPassword);

            var userId = await connection.ExecuteScalarAsync<long>(sql, parameters);
            if (userId > 0)
            {
                return new User
                {
                    Id = (int)userId,
                    Username = user.Username,
                    Password = null!
                };
            }
            return null;
        }
        #endregion


        #region QUERIES
        /// <summary>
        /// Get all users from the database.
        /// </summary>
        /// <param name="userFilter">user filter</param>
        /// <returns>Return users</returns>
        public async Task<PagedResult<User?>> GetAllUsers(UserFilter userFilter)
        {
            using var connection = dbConnectionFactory.CreateConnection();


            var sqlCount = @$"SELECT COUNT(*) FROM Users Where 1 = 1 ";

            var sb = new StringBuilder();
            sb.Append(@$"Select Id [{nameof(User.Id)}],
                                Username [{nameof(User.Username)}],
                                Password [{nameof(User.Password)}]
                         From Users where 1 = 1");
            var parameters = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(userFilter.Username))
            {
                sb.Append(" AND Username = @Username ");
                parameters.Add("Username", userFilter.Username);
            }

            var sortColumn = userFilter.SortBy.ToLower() switch
            {               
                _ => nameof(User.Username)
            };

            sb.Append(@$" order by {sortColumn} {userFilter.SortDir}");

            sb.Append($@" limit @PageSize offset @Skip ");
            parameters.Add("PageSize",userFilter.PageSize);
            parameters.Add("Skip",(userFilter.Page - 1) * userFilter.PageSize);

            using var multi = await connection.QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters);

            var totalCount = await multi.ReadFirstAsync<int>();
            var users = await multi.ReadAsync<User>();

            return new PagedResult<User?>
            {
                Items = users,
                TotalItems = totalCount,
                Page = userFilter.Page,
                PageSize = userFilter.PageSize
            };
        }        
        #endregion
    }
}
