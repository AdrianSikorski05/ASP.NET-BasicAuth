using Dapper;
using FluentAssertions;
using RestFullApiTest;

namespace RestFullApiTest.IntegrationsTests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async void AddUser_Should_InsertUserAndRole_When_DataIsValid()
        {
            // Arrange
            var connection = new SqliteConnectionFactory();

            var newUser = new CreateUserDto
            {
                Username = "testUser122",
                Password = "password123"
            };

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var userRepository = new UserRepository(connection);

                // Act
                var addedUser = await userRepository.AddUser(newUser, dbConnection, transaction);

                // Assert
                addedUser.Should().NotBeNull();
                addedUser.Id.Should().BeGreaterThan(0);
                addedUser.Username.Should().Be(newUser.Username);
                addedUser.Password.Should().BeNull();
                addedUser.Role.Should().Be("User");

                var inserted = await dbConnection.QuerySingleOrDefaultAsync<User>(
                    "SELECT u.Id, u.Username, u.Password, r.Role FROM Users u inner join Roles r on u.Id = r.UserId WHERE u.Id = @Id", new { Id = addedUser.Id }, transaction);

                inserted.Should().NotBeNull();
                inserted.Username.Should().Be(newUser.Username);
                inserted.Role.Should().Be("User");
                inserted.Password.Should().StartWith("$2a$");
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData("testuser", "123")]
        [InlineData("testUser1", "122")]
        public async void AddUser_Should_ThrowException_When_UserAlreadyExists(string username, string password)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            var existingUser = new CreateUserDto
            {
                Username = username,
                Password = password
            };

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            try
            {
                var userRepository = new UserRepository(connection);

                // Act
                Func<Task> act = async () => await userRepository.AddUser(existingUser, dbConnection, transaction);

                // Assert            
                await act.Should().ThrowAsync<Exception>().WithMessage("User with this username already exists.");
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(8, "Username")]
        [InlineData(9, "Username")]
        [InlineData(10, null)]
        public async void UpdateUser_Should_UpdateUser_When_DataIsValid(int id, string username)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();

            var userWhoWillBeUpdated = new UpdateUserDto
            {
                Id = id,
                Username = username
            };
            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            try
            {
                var userRepository = new UserRepository(connection);

                // Act
                var (updatedUser, status) = await userRepository.UpdateUser(userWhoWillBeUpdated, dbConnection, transaction);

                // Assert

                (updatedUser, status).updatedUser.Should().NotBeNull();
                (updatedUser, status).status.Should().NotBe(0);
                (updatedUser, status).updatedUser.Id.Should().Be(userWhoWillBeUpdated.Id);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(1, "test")]
        [InlineData(2, "test1")]
        public async void UpdateUser_Should_ThrowException_When_UserNotExists(int id, string username)
        {
            //  Arrange
            var connection = new SqliteConnectionFactory();
            var updateUserDto = new UpdateUserDto
            {
                Id = id,
                Username = username
            };

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            try
            {
                var userRepository = new UserRepository(connection);

                // Act

                Func<Task> act = async () => await userRepository.UpdateUser(updateUserDto, dbConnection, transaction);


                // Assert

                await act.Should().ThrowAsync<Exception>().WithMessage("User with this id not exists.");

            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public async void DeleteUser_Should_DeleteUser_When_UserExists(int id)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            var userToDelete = new DeleteUserDto
            {
                Id = id
            };

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var userRepository = new UserRepository(connection);
                // Act

                var result = await userRepository.DeleteUser(userToDelete, dbConnection, transaction);

                // Assert

                result.Should().Be(1);

                var deletedUser = await dbConnection.ExecuteScalarAsync<int>(
                    "select exists(select * from Users where Id = @Id)", new { Id = userToDelete.Id }, transaction);

                deletedUser.Should().Be(0);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void DeleteUser_Should_ThrowException_When_UserNotExists(int id)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            var userToDelete = new DeleteUserDto
            {
                Id = id
            };
            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var userRepository = new UserRepository(connection);

                // Act
                Func<Task> act = async () => await userRepository.DeleteUser(userToDelete, dbConnection, transaction);

                // Assert

                await act.Should().ThrowAsync<Exception>().WithMessage("User with this id not exists.");
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public async void DeleteUserById_Should_DeleteUser_When_UserExists(int id)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var userRepository = new UserRepository(connection);
                // Act

                var result = await userRepository.DeleteUserById(id, dbConnection, transaction);

                // Assert

                result.Should().Be(1);

                var deletedUser = await dbConnection.ExecuteScalarAsync<int>(
                    "select exists(select * from Users where Id = @Id)", new { Id = id }, transaction);

                deletedUser.Should().Be(0);
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void DeleteUserById_Should_ThrowException_When_UserNotExists(int id)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();

            using var dbConnection = connection.CreateConnection();
            dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var userRepository = new UserRepository(connection);

                // Act
                Func<Task> act = async () => await userRepository.DeleteUserById(id, dbConnection, transaction);

                // Assert

                await act.Should().ThrowAsync<Exception>().WithMessage("User with this id not exists.");
            }
            finally
            {
                transaction.Rollback();
            }
        }

        [Theory]
        [InlineData(1, 5, "testuser", nameof(User.Username), "desc")]
        public async void GetAllUsers_Should_ReturnAllUsers_When_Called(int page, int pageSize, string? username, string sortBy, string sortDir)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();
            var userRepository = new UserRepository(connection);
            var filter = new UserFilter
            {
                Page = page,
                PageSize = pageSize,
                Username = username,
                SortBy = sortBy,
                SortDir = sortDir
            };

            // Act
            var result = await userRepository.GetAllUsers(filter);

            // Assert
            result.Items.Should().NotBeNull();
            result.Items.Should().AllSatisfy(u => u.Username.Should().Be(username));
            if (sortDir == "desc")            
                result.Items.Should().BeInDescendingOrder(u => u.Username);
            else 
                result.Items.Should().BeInAscendingOrder(u => u.Username);
            result.Page.Should().Be(page);
            result.PageSize.Should().Be(pageSize);
            result.TotalItems.Should().BeGreaterThan(0);
            result.TotalPages.Should().BeGreaterThan(0);

        }

        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public async void GetUserById_Should_ReturnUser_When_UserExists(int id)
        { 
            // Arrange
            var connection = new SqliteConnectionFactory();            
            var userRepository = new UserRepository(connection);

            // Act
            var user = await userRepository.GetUserById(id);

            // Assert
            user.Should().NotBeNull();
            user.Role.Should().Be("User");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetUserById_Should_ThrowException_When_UserNotExists(int id)
        {
            // Arrange
            var connection = new SqliteConnectionFactory();           
            var userRepository = new UserRepository(connection);

            // Act
            Func<Task> act = async () => await userRepository.GetUserById(id);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("User with this id not exists.");
        }


        [Fact]
        public async void GetUserByUsername_Should_ReturnUser_When_UserExists() 
        { 
            // Arrange
            var connection = new SqliteConnectionFactory();           
            var userRepository = new UserRepository(connection);

            // Act
            var user = await userRepository.GetUserByUsername("testuser");

            // Assert
            user.Should().NotBeNull();
            user.Username.Should().Be("testuser");
            user.Role.Should().Be("User");
        }
    }
}