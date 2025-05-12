namespace RestFullApiTest
{
    public class Book
    {
        /// <summary>
        /// Constructor to initialize a new instance of the Book class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="genre"></param>
        /// <param name="publishedDate"></param>
        /// <param name="price"></param>
        /// <param name="stock"></param>
        public Book(int id, string title, string author, string genre, DateTime publishedDate, decimal price, int stock)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            PublishedDate = publishedDate;
            Price = price;
            Stock = stock;
        }

        public Book(){}
        /// <summary>
        /// Unique identifier for the book
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Author of the book
        /// </summary>
        public string Author { get; set; } = string.Empty;
        /// <summary>
        /// Genre of the book
        /// </summary>
        public string Genre { get; set; } = string.Empty;
        /// <summary>
        /// Date when the book was published
        /// </summary>
        public DateTime PublishedDate { get; set; }
        /// <summary>
        /// Price of the book
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Stock quantity of the book
        /// </summary>
        public int Stock { get; set; }
    }
}
