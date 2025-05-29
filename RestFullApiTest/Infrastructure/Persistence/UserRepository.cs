
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

            var sql2 = @$"Insert into roles(UserId, Role)
                          values(@UserId, @Role)";
            var userId = await connection.ExecuteScalarAsync<long>(sql, parameters);
            parameters.Add("Role", "User");
            parameters.Add("UserId", userId);
            await connection.ExecuteAsync(sql2, parameters);
            if (userId > 0)
            {
                return new User
                {
                    Id = (int)userId,
                    Username = user.Username,
                    Password = null!,
                    Role = "User"
                };
            }
            return null;
        }

        /// <summary>
        /// Update user in the database.
        /// </summary>
        /// <param name="updateUserDto">User who will updating</param>
        /// <returns>Return updated user</returns>
        public async Task<(User, int)> UpdateUser(UpdateUserDto updateUserDto)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sb = new StringBuilder();

            sb.Append("update Users set Id = @Id");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(updateUserDto.Username))
            {
                sb.Append(" , Username = @Username");
                parameters.Add("Username", updateUserDto.Username);
            }

            sb.Append(" where Id = @Id");
            parameters.Add("Id", updateUserDto.Id);

            var result = await connection.ExecuteAsync(sb.ToString(), parameters);
            if (result > 0)
            {
                return (await GetUserById(updateUserDto.Id), result);
            }
            return (null, 0);
        }

        /// <summary>
        /// Delete user by id.
        /// </summary>
        
        /// <param name="deleteUserDto">Obeject of user who will deleting</param>
        /// <returns>Return result</returns>        
        public Task<int> DeleteUser(DeleteUserDto deleteUserDto)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = $@"DELETE FROM Users WHERE Id = @Id";

            var parameters = new DynamicParameters();           
            parameters.Add("Id", deleteUserDto.Id);

            return connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return result</returns>       
        public Task<int> DeleteUserById(int id)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = $@"DELETE FROM Users WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return connection.ExecuteAsync(sql, parameters);
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
            parameters.Add("PageSize", userFilter.PageSize);
            parameters.Add("Skip", (userFilter.Page - 1) * userFilter.PageSize);

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

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return searched user</returns>
        public async Task<User?> GetUserById(int id)
        {
            using var connection = dbConnectionFactory.CreateConnection();

            var sql = $@"select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                r.Role [{nameof(User.Role)}]
                        from Users u
                        inner join Roles r on u.Id = r.UserId
                        where u.Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var result = await connection.QueryFirstOrDefaultAsync<User?>(sql, parameters);
            if (result is null)
            {
                throw new Exception("Dont found User.");
            }

            return result;
        }

        /// <summary>
        /// Get user by username.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Return object of user</returns>
        public Task<User?> GetUserByUsername(string username)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                u.Password [{nameof(User.Password)}],
                                r.Role [{nameof(User.Role)}]
                        from Users u
                        inner join Roles r on u.id = r.UserId
                        where Username = @Username";
            var parameters = new DynamicParameters();
            parameters.Add("Username", username);
            return connection.QueryFirstOrDefaultAsync<User?>(sql, parameters);
        }
       
        #endregion
    }
}
