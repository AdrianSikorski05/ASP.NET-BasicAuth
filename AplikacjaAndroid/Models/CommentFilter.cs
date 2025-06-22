namespace AplikacjaAndroid
{
    public class CommentFilter
    {
        /// <summary>
        /// Page number for pagination
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// The number of items to be displayed on each page
        /// </summary>
        public int PageSize { get; set; } = 8;

        public int BookId { get; set; }
        /// <summary>
        /// Sorting criteria for the book list
        /// </summary>
        public string? SortBy { get; set; } = "PublishedDate";
        /// <summary>
        /// The direction of sorting, either ascending or descending
        /// </summary>
        public string SortDir { get; set; } = "desc";
    }
}
