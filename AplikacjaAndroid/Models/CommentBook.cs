using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AplikacjaAndroid
{
    public partial class CommentBook : ObservableValidator
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string? Author { get; set; }
        [Required(ErrorMessage = "Content is required.")]
        public string? Content { get; set; }
        [Required(ErrorMessage = "Rate is required.")]
        public double? Rate { get; set; }
        public DateTime PublishedDate { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        bool _isOwner = false;

        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        public UpdateCommentBookDto MapToUpdateModel(CommentBook commentBook) 
        {
            return new UpdateCommentBookDto
            {
                Id = commentBook.Id,
                BookId = commentBook.BookId,
                UserId = commentBook.UserId,
                Author = commentBook.Author,
                Content = commentBook.Content,
                Rate = commentBook.Rate,
                PublishedDate = commentBook.PublishedDate
            };
        }
    }
}
