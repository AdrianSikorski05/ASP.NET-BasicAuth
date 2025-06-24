using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AplikacjaAndroid
{
    public partial class RegisterUser : ObservableValidator
    {
        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Username length must be between 4 and 12 characters.")]
        public string Username { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters, include upper, lower, digit and special char.")]
        public string Password { get; set; } = string.Empty;

        [JsonIgnore]
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        public bool ConfirmePasswordIsTheSame()
        {
            return Password.Equals(ConfirmPassword);
        }
    }
}
