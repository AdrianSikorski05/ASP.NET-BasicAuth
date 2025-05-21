namespace RestFullApiTest
{
    public class PagedResult<T>
    {
        /// <summary>
        /// Books list
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        /// <summary>
        /// Total number of items in the database
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Number of items to be displayed on each page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Total number of pages based on the total items and page size
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
