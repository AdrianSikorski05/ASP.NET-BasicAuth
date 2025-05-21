using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    /// <summary>
    /// Operation with books.
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    [Tags("Books")]
    public class BooksController(ILogger<BooksController> logger, IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Return all books with filtering, pagination and sorting.
        /// </summary>
        /// <param name="filter">Filters</param>
        /// <returns>List of books with metadata</returns>
        /// <response code="200">Return List of books</response>
        /// <response code="404">Dont found books</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpGet(Name = "Books")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetAll([FromQuery] BookFilter filter)
        {
            var result = await mediator.Send(new GetAllBooksQuery(filter));
            if (result == null)
            {
                logger.LogInformation("Books not founds.");
                return NotFound(ResponseResult.NotFound("Books not founds."));
            }
            else
            {
                logger.LogInformation($"Founds {result.Items.ToList().Count} books");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Get book by id.
        /// </summary>
        /// <param name="id">Id of book</param>
        /// <returns>Return book about fallen id.</returns>  
        /// <response code="200">Return book</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpGet("{id}", Name = "GetBookById")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                logger.LogError($"Id is not valid: {id}");
                return BadRequest(ResponseResult.BadRequest("Id is not valid"));
            }

            var result = await mediator.Send(new GetBookByIdQuery(id));
            if (result == null)
            {
                logger.LogError($"Book with id {id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {id} not found"));
            }
            else
            {
                logger.LogInformation($"Book with id {id} found");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Return response object result. </returns>
        /// <response code = "200"> Return response object with new book.</response>
        /// <response code = "400"> Return response object with information about not valid model.</response>        
        [HttpPost(Name = "AddBook")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> AddNewBook([FromBody] CreateBookDto book)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var newBook = await mediator.Send(new CreateBookCommand(book));
            if (newBook == null)
            {
                logger.LogError("Book not created");
                return BadRequest(ResponseResult.BadRequest("Book not created"));
            }
            else
            {
                logger.LogInformation($"Book {newBook.Title} created");
                return Ok(ResponseResult.Success("Book created", newBook));
            }
        }

        /// <summary>
        /// Update book in the database.
        /// </summary>
        /// <param name="updatedBook">Book who will updating</param>
        /// <returns>Return a updated book</returns>
        /// <response code="200">Return updated book</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPut(Name = "UpdateBook")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> UpdateBook([FromBody] UpdateBookDto updatedBook)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var uBook = await mediator.Send(new UpdateBookCommand(updatedBook));
            if (uBook.Item2 == 0)
            {
                logger.LogError($"Book with id {updatedBook.Id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {updatedBook.Id} not found"));
            }
            else
            {
                logger.LogInformation($"Book with id {uBook.Item1.Id} updated");
                return Ok(ResponseResult.Success("OK", uBook.Item1));
            }          
        }

        /// <summary>
        /// Delete book from the database.
        /// </summary>
        /// <param name="deleteBookDto">Object book who will deleting</param>
        /// <returns>Id book who was deleted</returns>
        /// <response code="200">Sucess</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpDelete(Name = "DeleteBook")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> DeleteBook([FromBody] DeleteBookDto deleteBookDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }
            var result = await mediator.Send(new DeleteBookCommand(deleteBookDto));
            if (result == 0)
            {
                logger.LogError($"Book with id {deleteBookDto.Id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {deleteBookDto.Id} not found"));
            }
            else
            {
                logger.LogInformation($"Book with id {deleteBookDto.Id} deleted");
                return Ok(ResponseResult.Success("OK", $"Book with id {deleteBookDto.Id} deleted"));
            }
        }
    }
}
