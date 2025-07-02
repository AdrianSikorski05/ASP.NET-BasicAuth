
using System.Data;

namespace RestFullApiTest
{
    public interface IUserRepository
    {
        Task<User?> AddUser(CreateUserDto user, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<PagedResult<User>> GetAllUsers(UserFilter userFilter, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<User?> GetUserById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<User?> GetUserByUsername(string username, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<(User,int)> UpdateUser(UpdateUserDto updateUserDto, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int> DeleteUser(DeleteUserDto deleteUserDto, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int> DeleteUserById(int id, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<UserConfig?> GetUserConfig(int userId, IDbConnection connection = null, IDbTransaction? transaction = null);
    }
}
