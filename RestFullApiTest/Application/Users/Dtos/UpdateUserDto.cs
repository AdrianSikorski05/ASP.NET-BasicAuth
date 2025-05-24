using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
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
        public string Username { get; set; }
    }
}
