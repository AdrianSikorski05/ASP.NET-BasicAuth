using System.ComponentModel.DataAnnotations;

namespace AplikacjaAndroid
{
    public class UpdateCommentBookDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "BookId is required.")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public string? Author { get; set; }
        public string? Content { get; set; }
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        public double? Rate { get; set; }
        [Required(ErrorMessage = "PublishedDate is required.")]
        public DateTime PublishedDate { get; set; }
    }
}
