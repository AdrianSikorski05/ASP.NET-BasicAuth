using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class ActivityBook
    {
        [Required(ErrorMessage = "User id is required.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Book is required.")]
        public Book Book { get; set; }
        [Required(ErrorMessage = "Status is required.")]
        public BookStatus Status { get; set; }
    }
}
