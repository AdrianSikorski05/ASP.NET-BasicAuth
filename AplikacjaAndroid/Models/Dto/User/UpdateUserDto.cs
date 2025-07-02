using System.ComponentModel.DataAnnotations;

namespace AplikacjaAndroid
{
    public class UpdateUserDto
    {
        /// <summary>
        /// Id of the user to update.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }
        /// <summary>
        /// Username of the user to update.
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Password for the user
        /// </summary>        
        public string? Password { get; set; }
        /// <summary>
        /// User configuration details.
        /// </summary>
        public UserConfig? UserConfig { get; set; }
        /// <summary>
        /// Is user enabled?
        /// </summary>
        public bool Enabled { get; set; }
    }
}
