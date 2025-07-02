using CommunityToolkit.Mvvm.ComponentModel;

namespace AplikacjaAndroid
{
    public partial class User : ObservableValidator
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username of the user
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Password of the user
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Confirm password for the user
        /// </summary>
        public string? ConfirmPassword { get; set; }
        /// <summary>
        /// Role of the user
        /// </summary>
        public string? Role { get; set; }
        /// <summary>
        /// User configuration details
        /// </summary>
        [ObservableProperty]
        UserConfig _userConfig;
        /// <summary>
        /// Created date of the user account
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Is user enabled?
        /// </summary>
        public bool Enabled { get; set; }

        public bool ConfirmePasswordIsTheSame()
        {
            return Password.Equals(ConfirmPassword);
        }
    }
}
