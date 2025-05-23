using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    public class DeleteBookByIdDto
    {
        /// <summary>
        /// Id of book
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        [FromRoute(Name = "id")]
        public int Id { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public DeleteBookByIdDto(int id)
        {
            Id = id;
        }
        public DeleteBookByIdDto(){}
    }
}
