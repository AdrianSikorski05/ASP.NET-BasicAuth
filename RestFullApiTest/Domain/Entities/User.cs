﻿using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class User
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        [Required(ErrorMessage = "Id is required.")]
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
        /// Role of the user
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// User configuration details
        /// </summary>
        public UserConfig UserConfig { get; set; }
        /// <summary>
        /// Created date of the user account
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Is user enabled?
        /// </summary>
        public bool Enabled { get; set; }

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

        public static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };
        }
    }
}
