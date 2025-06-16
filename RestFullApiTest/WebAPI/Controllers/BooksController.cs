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
    [Route("api/[controller]")]
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
        [HttpGet("books")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetAllBooks([FromQuery] BookFilter filter)
        {
            var result = await mediator.Send(new GetAllBooksQuery(filter));
            if (result.Items.ToList().Count <= 0)
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
        [HttpGet("{id}", Name = "getBookById")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetBookById([FromRoute] GetBookByIdDto getBookByIdDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Id is not valid: {getBookByIdDto.Id}");
                return BadRequest(ResponseResult.BadRequest("Id is not valid"));
            }

            var result = await mediator.Send(new GetBookByIdQuery(getBookByIdDto.Id));
            if (result == null)
            {
                logger.LogError($"Book with id {getBookByIdDto.Id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {getBookByIdDto.Id} not found"));
            }
            else
            {
                logger.LogInformation($"Book with id {result.Id} found");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="createBookDto"></param>
        /// <returns>Return response object result. </returns>
        /// <response code = "200"> Return response object with new book.</response>
        /// <response code = "400"> Return response object with information about not valid model.</response>        
        [HttpPost("addBook")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> AddNewBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var newBook = await mediator.Send(new CreateBookCommand(createBookDto));
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
        /// <param name="updatedBookDto">Book who will updating</param>
        /// <returns>Return a updated book</returns>
        /// <response code="200">Return updated book</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPut("updateBook")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> UpdateBook([FromBody] UpdateBookDto updatedBookDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var uBook = await mediator.Send(new UpdateBookCommand(updatedBookDto));
            if (uBook.Item2 == 0)
            {
                logger.LogError($"Book with id {updatedBookDto.Id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {updatedBookDto.Id} not found"));
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
        /// <param name="deleteBookDto">Object of book who will deleting</param>
        /// <returns>Id book who was deleted</returns>
        /// <response code="200">Sucess</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpDelete("deleteBook")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Delete book from the database by id.
        /// </summary>
        /// <param name="deleteBookByIdDto">Object of book who will deleting</param>
        /// <returns></returns>
        /// <response code="200">Sucess</response>
        /// <response code="404">Dont found book</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpDelete("{id}", Name = "deleteBookById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> DeleteBookById([FromRoute] DeleteBookByIdDto deleteBookByIdDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }
            var result = await mediator.Send(new DeleteBookByIdCommand(deleteBookByIdDto));
            if (result == 0)
            {
                logger.LogError($"Book with id {deleteBookByIdDto.Id} not found");
                return NotFound(ResponseResult.NotFound($"Book with id {deleteBookByIdDto.Id} not found"));
            }
            else
            {
                logger.LogInformation($"Book with id {deleteBookByIdDto.Id} deleted");
                return Ok(ResponseResult.Success("OK", $"Book with id {deleteBookByIdDto.Id} deleted"));
            }
        }


        [HttpPost("getBookWithActivityStatusByUser")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetBookWithActivityStatusByUser([FromBody] DataStatusBookWithUserIdDto data)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }
            var result = await mediator.Send(new GetBookWithActivityStatusByUserQuery(data));
            if (result.Count <= 0)
            {
                logger.LogInformation($"Books with activity status {data.StatusBook} not found for user {data.UserId}");
                return NotFound(ResponseResult.NotFound($"Books with activity status {data.StatusBook} not found for user {data.UserId}"));
            }
            else
            {
                logger.LogInformation($"Founds {result.Count} books with activity status {data.StatusBook} for user {data.UserId}");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        [HttpPost("updateBookActivityStatus")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> UpdateBookActivityStatus([FromBody] ActivityBook activityBook)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }
            var result = await mediator.Send(new UpdateBookActivityStatusCommand(activityBook));
            if (result)
            {
                logger.LogInformation($"Book activity status updated for user {activityBook.UserId} and book {activityBook.Book.Id}");
                return Ok(ResponseResult.Success("OK", "Book activity status updated"));
            }
            else
            {
                logger.LogError($"Failed to update book activity status for user {activityBook.UserId} and book {activityBook.Book.Id}");
                return BadRequest(ResponseResult.BadRequest("Failed to update book activity status"));
            }
        }
    }
}
