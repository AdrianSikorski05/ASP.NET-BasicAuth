﻿using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class UserDto
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; } = null!;
    }
}
