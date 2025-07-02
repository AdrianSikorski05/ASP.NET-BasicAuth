
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
            var tran = transaction ?? connection.BeginTransaction();
            try
            {
                var checkUserExistsQuery = @$"select EXISTS(select * from Users where Username = @Username)";
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);

                bool exists = await connection.ExecuteScalarAsync<bool>(checkUserExistsQuery, parameters, tran);
                if (exists)
                {
                    throw new Exception("User with this username already exists.");
                }

                var insertUserQuery = @$"INSERT INTO Users (Username, Password, CreatedAt, Enabled)
                        VALUES (@Username, @Password, @CreatedAt, @Enabled);
                        SELECT last_insert_rowid();";

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                parameters.Add("Password", hashedPassword);
                parameters.Add("CreatedAt", user.CreatedAt);
                parameters.Add("Enabled", 1);


                var insertRoleQuery = @$"Insert into roles(UserId, Role)
                          values(@UserId, @Role)";
                var userId = await connection.ExecuteScalarAsync<long>(insertUserQuery, parameters, tran);
                parameters.Add("Role", "User");
                parameters.Add("UserId", userId);
                await connection.ExecuteAsync(insertRoleQuery, parameters, tran);

                var insertUserConfigQuery = @$"INSERT INTO UserConfig (UserId, Name, Surename, Email, PhoneNumber, AvatarColor, AvatarImage, ThemeApp)
                        VALUES (@UserId, @Name, @Surename, @Email, @PhoneNumber, @AvatarColor, @AvatarImage, @ThemeApp);";

                parameters.Add("Name", string.IsNullOrWhiteSpace(user.UserConfig.Name) ? null : user.UserConfig.Name);
                parameters.Add("Surename", string.IsNullOrWhiteSpace(user.UserConfig.Surename) ? null : user.UserConfig.Surename);
                parameters.Add("Email", string.IsNullOrWhiteSpace(user.UserConfig.Email) ? null : user.UserConfig.Email);
                parameters.Add("PhoneNumber", string.IsNullOrWhiteSpace(user.UserConfig.PhoneNumber) ? null : user.UserConfig.PhoneNumber);
                parameters.Add("AvatarColor", string.IsNullOrWhiteSpace(user.UserConfig.SelectedColor) ? null : user.UserConfig.SelectedColor);
                parameters.Add("AvatarImage", user.UserConfig.AvatarBytes == null || user.UserConfig.AvatarBytes.Length == 0 ? null : user.UserConfig.AvatarBytes);
                parameters.Add("ThemeApp", "Light");


                await connection.ExecuteAsync(insertUserConfigQuery, parameters, tran);

                if (userId > 0)
                {
                    tran.Commit();
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
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while adding user to the database.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                {
                    connection?.Dispose();
                }
                if (transaction == null)
                    tran.Dispose();
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
            var tran = transaction ?? connection.BeginTransaction();
            try
            {
                // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", updateUserDto.Id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, tran);
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

                if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
                    sb.Append(" , Password = @Password");
                    parameters.Add("Password", hashedPassword);
                }

                if (updateUserDto.UserConfig != null)
                {
                    await UpdateUserConfig(updateUserDto, connection, tran);
                }

                sb.Append(" , Enabled = @Enabled");
                parameters.Add("Enabled", updateUserDto.Enabled);

                sb.Append(" where Id = @Id");

                var result = await connection.ExecuteAsync(sb.ToString(), parameters, tran);
                if (result > 0)
                {
                    var finalResult = (await GetUserById(updateUserDto.Id, connection, tran), result);
                    tran.Commit();

                    return finalResult;
                }
                return (null, result);

            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while updating user in the database.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
                if (transaction == null)
                    tran.Dispose();
            }
        }

        private async Task<bool> UpdateUserConfig(UpdateUserDto updateUserDto, IDbConnection? connection, IDbTransaction? transaction)
        {
            try
            {
                var sql = @"UPDATE UserConfig SET
                                Name = @Name,
                                Surename = @Surename, 
                                Email = @Email, 
                                PhoneNumber = @PhoneNumber, 
                                AvatarColor = @AvatarColor, 
                                AvatarImage = @AvatarImage,
                                ThemeApp = @ThemeApp
                            WHERE UserId = @UserId";

                var parameters = new DynamicParameters();

                parameters.Add("Name", updateUserDto.UserConfig.Name);
                parameters.Add("Surename", updateUserDto.UserConfig.Surename);
                parameters.Add("Email", updateUserDto.UserConfig.Email);
                parameters.Add("PhoneNumber", updateUserDto.UserConfig.PhoneNumber);
                parameters.Add("AvatarColor", updateUserDto.UserConfig.SelectedColor);
                parameters.Add("AvatarImage", updateUserDto.UserConfig.AvatarBytes);
                parameters.Add("ThemeApp", updateUserDto.UserConfig.Theme);
                parameters.Add("UserId", updateUserDto.Id);


                var result = await connection.ExecuteAsync(sql, parameters, transaction);
                return result > 0;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating userConfig.", ex);
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
            var tran = transaction ?? connection.BeginTransaction();
            try
            {   // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", deleteUserDto.Id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, tran);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }

                var sql = $@"DELETE FROM Users WHERE Id = @Id";

                var result = await connection.ExecuteAsync(sql, parameters, tran);
                tran.Commit();

                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while deleting user from the database.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
                if (transaction == null)
                    tran.Dispose();
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
            var tran = transaction ?? connection.BeginTransaction();
            try
            {
                // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, tran);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }

                var sql = $@"DELETE FROM Users WHERE Id = @Id";
                var result = await connection.ExecuteAsync(sql, parameters, tran);

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while deleting user from the database.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
                if (transaction == null)
                    tran.Dispose();
            }
        }

        public async Task<int> MakeAccountDisabled(int id, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();
            var tran = transaction ?? connection.BeginTransaction();
            try
            {
                // Check if user exists
                var checkUserNotExistsQuery = @$"select exists(select * from Users where Id = @Id)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id);
                var exists = await connection.ExecuteScalarAsync<bool>(checkUserNotExistsQuery, parameters, tran);
                if (!exists)
                {
                    throw new Exception("User with this id not exists.");
                }

                var sql = $@"update Users set Enabled = 0 where Id = @Id";
                var result = await connection.ExecuteAsync(sql, parameters, tran);

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while disabled user.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
                if (transaction == null)
                    tran.Dispose();
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
            var tran = transaction ?? connection.BeginTransaction();
            try
            {
                var sqlCount = @$"SELECT COUNT(*) FROM Users Where 1 = 1 ";

                var sb = new StringBuilder();
                sb.Append(@$"Select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                u.Password [{nameof(User.Password)}],
                                r.Role [{nameof(User.Role)}],
                                u.CreatedAt [{nameof(User.CreatedAt)}],
                                u.Enabled [{nameof(User.Enabled)}]
                         From Users u 
                         inner join Roles r on u.Id = r.UserId
                         where 1 = 1");
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

                using var multi = await connection.QueryMultipleAsync(sqlCount + ";" + sb.ToString(), parameters, tran);

                var totalCount = await multi.ReadFirstAsync<int>();
                var users = await multi.ReadAsync<User>();

                tran.Commit();
                return new PagedResult<User?>
                {
                    Items = users,
                    TotalItems = totalCount,
                    Page = userFilter.Page,
                    PageSize = userFilter.PageSize
                };
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Error while getting users from the database.", ex);
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
                if (transaction == null)
                    tran.Dispose();
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
                                r.Role [{nameof(User.Role)}],
                                u.CreatedAt [{nameof(User.CreatedAt)}],
                                u.Enabled [{nameof(User.Enabled)}]
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

                result.UserConfig = await GetUserConfig(id, connection, transaction);

                return result;
            }
            finally
            {
                if (shouldDisposeConnection)
                    connection?.Dispose();
            }
        }

        public async Task<UserConfig?> GetUserConfig(int userId, IDbConnection connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
                var sql = @$"SELECT Id [{nameof(UserConfig.Id)}],
                                UserId [{nameof(UserConfig.UserId)}],
                                Name [{nameof(UserConfig.Name)}],
                                Surename [{nameof(UserConfig.Surename)}],
                                Email [{nameof(UserConfig.Email)}],
                                PhoneNumber [{nameof(UserConfig.PhoneNumber)}],
                                AvatarColor [{nameof(UserConfig.SelectedColor)}],
                                AvatarImage [{nameof(UserConfig.AvatarBytes)}],
                                ThemeApp [{nameof(UserConfig.Theme)}]
                        FROM UserConfig
                        WHERE UserId = @UserId";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);

                var result = await connection.QueryFirstOrDefaultAsync<UserConfig?>(sql, parameters, transaction);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting user by id from the database.", ex);
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
        public async Task<User?> GetUserByUsername(string username, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var shouldDisposeConnection = connection == null;
            connection ??= dbConnectionFactory.CreateConnection();

            try
            {
                var sql = @$"select u.Id [{nameof(User.Id)}],
                                u.Username [{nameof(User.Username)}],
                                u.Password [{nameof(User.Password)}],
                                r.Role [{nameof(User.Role)}],
                                u.CreatedAt [{nameof(User.CreatedAt)}],
                                u.Enabled [{nameof(User.Enabled)}]
                        from Users u
                        inner join Roles r on u.id = r.UserId
                        where Username = @Username";
                var parameters = new DynamicParameters();
                parameters.Add("Username", username);

                var result = await connection.QueryFirstOrDefaultAsync<User?>(sql, parameters, transaction);

                return result;
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
