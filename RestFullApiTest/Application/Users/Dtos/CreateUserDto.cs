using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class CreateUserDto
    {
        /// <summary>
        /// Username for the user
        /// </summary>
        [Required]
        public string? Username { get; set; }
        /// <summary>
        /// Password for the user
        /// </summary>
        [Required]
        public string? Password { get; set; }
        public UserConfig? UserConfig { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
    }
}
