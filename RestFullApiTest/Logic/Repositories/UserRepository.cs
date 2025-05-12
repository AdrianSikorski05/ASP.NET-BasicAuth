
using Dapper;

namespace RestFullApiTest
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UserRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<User?> AddUser(CreateUserDto user)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
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

        public async Task<IEnumerable<User?>> GetAllUsers()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"Select Id [{nameof(User.Id)}],
                                Username [{nameof(User.Username)}],
                                Password [{nameof(User.Password)}]
                         From Users";
            return await connection.QueryAsync<User>(sql);
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = @$"Select Id [{nameof(User.Id)}],
                                Username [{nameof(User.Username)}],
                                Password [{nameof(User.Password)}]
                         From Users
                         Where Username = @Username";
            var parameters = new DynamicParameters();
            parameters.Add("Username", username);
            return await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }
    }
}
