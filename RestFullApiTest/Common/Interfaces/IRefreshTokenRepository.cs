namespace RestFullApiTest
{
    public interface IRefreshTokenRepository
    {
        Task SaveToken(int userId, string token, DateTime expiresAt);
        Task<RefreshToken?> Get(string token);
        Task Revoke(int tokenId);
    }
}
