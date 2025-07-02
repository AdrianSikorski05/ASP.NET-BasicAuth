using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class RegisterUser
    {
        [Required]
        public string? Username { get; set; } 
        [Required]
        public string? Password { get; set; }
        public UserConfig? UserConfig { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
    }
}
