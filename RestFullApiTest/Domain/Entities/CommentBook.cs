namespace RestFullApiTest
{
    public class CommentBook
    {
        public int id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string? Author { get; set; }
        public string? Content { get; set; }
        public double? Rate { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
