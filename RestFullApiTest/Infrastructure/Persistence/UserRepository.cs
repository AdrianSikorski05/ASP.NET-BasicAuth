
using System.Data;
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
        public async Task<User?> AddUser(CreateUserDto user, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
                var checkUserExistsQuery = @$"select EXISTS(select * from Users where Username = @Username)";
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);

                bool exists = await connection.ExecuteScalarAsync<bool>(checkUserExistsQuery, parameters, transaction);
                if (exists)
                {
                    throw new Exception("User with this username already exists.");
                }

                var insertUserQuery = @$"INSERT INTO Users (Username, Password)
                        VALUES (@Username, @Password);
                        SELECT last_insert_rowid();";

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                parameters.Add("Password", hashedPassword);

                var insertRoleQuery = @$"Insert into roles(UserId, Role)
                          values(@UserId, @Role)";
                var userId = await connection.ExecuteScalarAsync<long>(insertUserQuery, parameters, transaction);
                parameters.Add("Role", "User");
                parameters.Add("UserId", userId);
                await connection.ExecuteAsync(insertRoleQuery, parameters, transaction);
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
            finally
            {
                if (shouldDisposeConnection)
                {
                    connection?.Dispose();
                }
            }
        }

        /// <summary>
        /// Update user in the database.
        /// </summary>
        /// <param name="updateUserDto">User who will updating</param>
        /// <returns>Return updated user</returns>
        public async Task<(User, int)> UpdateUser(UpdateUserDto updateUserDto, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
                // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", updateUserDto.Id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, transaction);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }
                var sb = new StringBuilder();

                sb.Append("update Users set Id = @Id");


                if (!string.IsNullOrWhiteSpace(updateUserDto.Username))
                {
                    sb.Append(" , Username = @Username");
                    parameters.Add("Username", updateUserDto.Username);
                }

                sb.Append(" where Id = @Id");
                parameters.Add("Id", updateUserDto.Id);

                var result = await connection.ExecuteAsync(sb.ToString(), parameters, transaction);
                if (result > 0)
                {
                    return (await GetUserById(updateUserDto.Id,connection,transaction), result);
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
        /// Delete user by id.
        /// </summary>      
        /// <param name="deleteUserDto">Obeject of user who will deleting</param>
        /// <returns>Return result</returns>        
        public async Task<int> DeleteUser(DeleteUserDto deleteUserDto, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {   // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", deleteUserDto.Id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, transaction);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }

                var sql = $@"DELETE FROM Users WHERE Id = @Id";
              
                return await connection.ExecuteAsync(sql, parameters, transaction);
            }
            finally
            {
                if (shouldDisposeConnection)                
                    connection?.Dispose();               
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return result</returns>       
        public async Task<int> DeleteUserById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, transaction);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }

                var sql = $@"DELETE FROM Users WHERE Id = @Id";
             
                return await connection.ExecuteAsync(sql, parameters, transaction);
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
        /// Get all users from the database.
        /// </summary>
        /// <param name="userFilter">user filter</param>
        /// <returns>Return users</returns>
        public async Task<PagedResult<User?>> GetAllUsers(UserFilter userFilter, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
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

                using var multi = await connection.QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters, transaction);

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
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return searched user</returns>
        public async Task<User?> GetUserById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sql = $@"select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                r.Role [{nameof(User.Role)}]
                        from Users u
                        inner join Roles r on u.Id = r.UserId
                        where u.Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id);

                var result = await connection.QueryFirstOrDefaultAsync<User?>(sql, parameters, transaction);
                if (result is null)
                {
                    throw new Exception("User with this id not exists.");
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
        /// Get user by username.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Return object of user</returns>
        public Task<User?> GetUserByUsername(string username, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            try
            {
                var sql = @$"select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                u.Password [{nameof(User.Password)}],
                                r.Role [{nameof(User.Role)}]
                        from Users u
                        inner join Roles r on u.id = r.UserId
                        where Username = @Username";
                var parameters = new DynamicParameters();
                parameters.Add("Username", username);
                return connection.QueryFirstOrDefaultAsync<User?>(sql, parameters, transaction);
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
