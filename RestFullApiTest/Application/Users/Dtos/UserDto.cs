using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class UserDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }
        public string Username { get; set; } = null!;
    }
}
