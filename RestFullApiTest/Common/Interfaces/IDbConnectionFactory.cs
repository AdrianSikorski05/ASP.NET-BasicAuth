using System.Data;

namespace RestFullApiTest
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
