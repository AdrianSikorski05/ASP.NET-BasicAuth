
namespace RestFullApiTest
{
    public interface IUserRepository
    {
        Task<User?> AddUser(CreateUserDto user);
        Task<PagedResult<User>> GetAllUsers(UserFilter userFilter);
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByUsername(string username);
        Task<(User,int)> UpdateUser(UpdateUserDto updateUserDto);
        Task<int> DeleteUser(DeleteUserDto deleteUserDto);
        Task<int> DeleteUserById(int id);
    }
}
