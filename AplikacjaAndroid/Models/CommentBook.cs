using AplikacjaAndroid.Models.Dto.Comment;
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
        [ObservableProperty]
        byte[]? _avatarImage;
        partial void OnAvatarImageChanged(byte[]? value)
        {
            Avatar = value != null ? ImageSource.FromStream(() => new MemoryStream(value)) : null;
        }

        [ObservableProperty]
        string? _name;
        partial void OnNameChanged(string? value)
        {
            if (value != null)
            {
                Initials = Name?.FirstOrDefault().ToString().ToUpper() + Surename?.FirstOrDefault().ToString().ToUpper();
            }
            else
            {
                Initials = null;
            }
        }

        [ObservableProperty]
        string? _surename;
        partial void OnSurenameChanged(string? value)
        {
            if (value != null)
            {
                Initials = Name?.FirstOrDefault().ToString().ToUpper() + Surename?.FirstOrDefault().ToString().ToUpper();
            }
            else
            {
                Initials = null;
            }
        }
        public string? AvatarColor { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        ImageSource _avatar;

        [JsonIgnore]
        [ObservableProperty]
        string? _initials;

        [JsonIgnore]
        [ObservableProperty]
        bool _isOwner = false;

        [JsonIgnore]
        public ActionComment ActionComment { get; set; } = ActionComment.Default;

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
