using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    public class GetUserById
    {
        /// <summary>
        /// Id of the user to retrieve.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}
