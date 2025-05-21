namespace RestFullApiTest
{
    public class User
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password of the user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Constructor to initialize a new instance of the User class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public User(int id, string username,  string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }
        public User(){}
    }
}
