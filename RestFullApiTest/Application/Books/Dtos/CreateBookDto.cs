using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class CreateBookDto
    {
        public CreateBookDto( string title, string author, string genre, decimal price, int stock)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Price = price;
            Stock = stock;
        }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = null!;
        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
