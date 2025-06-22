using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplikacjaAndroid
{
    public partial class CreateCommentBookDto : ObservableValidator
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string? Author { get; set; }
        [Required(ErrorMessage = "Content is required.")]
        [ObservableProperty]
        string? _content;
        [Required(ErrorMessage = "Rate is required.")]
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        [ObservableProperty]
        double? _rate = 0;
        public DateTime PublishedDate { get; set; } = DateTime.Now;


        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;
        }       
    }
}
