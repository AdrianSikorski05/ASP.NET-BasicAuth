using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    public class DeleteUserDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }
        /// <summary>
        /// Username of user
        /// </summary>
        public string? Username { get; set; }
    }
}
