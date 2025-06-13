using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplikacjaAndroid
{
    public class LoginUser : ObservableValidator
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public LoginUser(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public LoginUser() { }
        public override string ToString()
        {
            return $"Username: {Username}, Password: {Password}";
        }

        public bool IsValid()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }
}
