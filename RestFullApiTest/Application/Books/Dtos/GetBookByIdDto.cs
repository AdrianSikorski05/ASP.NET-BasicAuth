using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    public class GetBookByIdDto
    {
        /// <summary>
        /// Book Id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        [FromRoute(Name = "id")]
        public int Id { get; set; }
        public GetBookByIdDto(int id)
        {
            Id = id;
        }
        public GetBookByIdDto(){}
    }
}
