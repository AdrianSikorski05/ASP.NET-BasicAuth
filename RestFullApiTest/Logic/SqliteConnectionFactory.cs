using System.Data;
using Microsoft.Data.Sqlite;

namespace RestFullApiTest
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SqliteConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection");
        }
        public IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open(); 
            return connection;
        }
    }
}
