using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class BookDto
    {
        /// <summary>
        /// Id of the book.
        /// </summary>
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }
        /// <summary>
        /// Title of the book.
        /// </summary>
        public string Title { get; set; } 
        /// <summary>
        /// Author of the book.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Genre of the book.
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// Published date of the book.
        /// </summary>
        public DateTime PublishedDate { get; set; }
        /// <summary>
        /// Price of the book.
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Stock of the book.
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// Image of book
        /// </summary>
        public byte[]? Image { get; set; }
        /// <summary>
        /// Description of the book
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Average rating of the book, nullable if not rated
        /// </summary>
        public double? AverageRating { get; set; }
    }
}
