﻿using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class LoginDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string? Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string? Password { get; set; }


    }
}
