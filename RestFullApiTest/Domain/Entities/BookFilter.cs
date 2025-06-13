namespace RestFullApiTest
{
    public class BookFilter
    {
        /// <summary>
        /// Page number for pagination
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// The number of items to be displayed on each page
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// The title of the book
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// The author of the book
        /// </summary>
        public string? Author { get; set; }
        /// <summary>
        /// The genre of the book
        /// </summary>
        public string? Genre { get; set; }
        /// <summary>
        /// Sorting criteria for the book list
        /// </summary>
        public string SortBy { get; set; } = "Title";
        /// <summary>
        /// The direction of sorting, either ascending or descending
        /// </summary>
        public string SortDir { get; set; } = "asc";
        

    }
}
