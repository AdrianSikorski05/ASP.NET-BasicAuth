namespace RestFullApiTest
{
    public interface IUserService
    {
        Task<User> AddUser(CreateUserDto user);
        Task<IEnumerable<User?>> GetAllUsers();
        Task<User?> GetUserByUsername(string username);
    }
}
