
using Dapper;

namespace RestFullApiTest
{
    public class RefreshTokenRepository(IDbConnectionFactory dbConnectionFactory) : IRefreshTokenRepository
    {
        public Task<RefreshToken?> Get(string token)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"SELECT Id [{nameof(RefreshToken.Id)}],
                                UserId [{nameof(RefreshToken.UserId)}],
                                Token [{nameof(RefreshToken.Token)}],
                                ExpiresAt [{nameof(RefreshToken.ExpiresAt)}],
                                Revoked [{nameof(RefreshToken.Revoked)}]
                        FROM RefreshTokens 
                        WHERE Token = @Token";

            var parameters = new DynamicParameters();
            parameters.Add("Token", token);

            return connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, parameters);
        }

        public Task Revoke(int tokenId)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"UPDATE RefreshTokens 
                        SET Revoked = 1 
                        WHERE Id = @TokenId";
            var parameters = new DynamicParameters();
            parameters.Add("TokenId", tokenId);
            return connection.ExecuteAsync(sql, parameters);
        }

        public Task SaveToken(int userId, string token, DateTime expiresAt)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var sql = @$"INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, Revoked)
                        VALUES (@UserId, @Token, @ExpiresAt, 0)";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("Token", token);
            parameters.Add("ExpiresAt", expiresAt);
            return connection.ExecuteAsync(sql, parameters);
        }
    }
}
