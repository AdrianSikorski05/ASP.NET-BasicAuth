
namespace RestFullApiTest
{
    public interface IUserRepository
    {
        Task<User?> AddUser(CreateUserDto user);
        Task<PagedResult<User>> GetAllUsers(UserFilter userFilter);

    }
}
