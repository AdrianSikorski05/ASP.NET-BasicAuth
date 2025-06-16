using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class DataStatusBookWithUserIdDto
    {
        [Required(ErrorMessage ="User id is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Status book is required.")]
        public string? StatusBook { get; set; }
    }
}
