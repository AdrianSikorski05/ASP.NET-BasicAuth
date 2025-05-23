using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class BookDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
