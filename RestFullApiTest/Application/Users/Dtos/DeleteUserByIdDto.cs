using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    public class DeleteUserByIdDto
    {
        /// <summary>
        /// Id of the user to delete.
        /// </summary>
        [Required]
        [FromRoute(Name = "id")]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }
    }
}
