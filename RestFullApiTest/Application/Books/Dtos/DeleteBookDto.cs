using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class DeleteBookDto
    {
        /// <summary>
        /// Constructor for UpdateBookDto.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="title">title</param>
        /// <param name="author">author</param>
        /// <param name="genre">genre</param>
        /// <param name="price">price</param>
        /// <param name="stock">stock</param>
        public DeleteBookDto(int id)
        {
            Id = id;
        }

        /// <summary>
        /// The unique identifier for the book.
        /// </summary>
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }
        /// <summary>
        /// The title of the book.
        /// </summary>
        public string? Title { get; set; } = null!;

        /// <summary>
        /// The author of the book.
        /// </summary>
        public string? Author { get; set; } = null!;

        /// <summary>
        /// The genre of the book.
        /// </summary>
        public string? Genre { get; set; } = null!;

        /// <summary>
        /// The price of the book.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// The stock of the book.
        /// </summary>
        public int? Stock { get; set; }
    }
}
