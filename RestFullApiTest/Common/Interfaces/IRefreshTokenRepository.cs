using System.Data;

namespace RestFullApiTest
{
    public interface IRefreshTokenRepository
    {
        Task SaveToken(int userId, string token, DateTime expiresAt, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<RefreshToken?> Get(string token, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task Revoke(int tokenId, IDbConnection? connection = null, IDbTransaction? transaction = null);
    }
}
