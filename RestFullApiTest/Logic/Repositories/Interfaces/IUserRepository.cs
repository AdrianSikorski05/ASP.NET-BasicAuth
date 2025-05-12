namespace RestFullApiTest
{
    public interface IUserRepository
    {
        Task<User?> AddUser(CreateUserDto user);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserByUsername(string username);

    }
}
