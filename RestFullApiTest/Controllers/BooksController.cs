using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestFullApiTest.Logic.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    [ApiController]
    public class BooksController(IBookService service, ILogger<BooksController> logger) : ControllerBase
    {

        /// <summary>
        /// Pobiera wszystkie książki
        /// </summary>
        /// <returns>zwraca obiekt Ok</returns>
        [HttpGet(Name = "Books")]
        public async Task<ActionResult<ResponseResult>> Get()
        {
            var books = await service.GetAllAsync();
            if (books == null)
            {
                logger.LogInformation("Books not founds.");
                return NotFound(ResponseResult.NotFound("Books not founds."));
            }
            else
            {
                logger.LogInformation($"Founds {books.ToList().Count} books");
                return Ok(ResponseResult.Success("OK", books));
            }

        }

        //// GET: api/books/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<BookDto>> Get(int id)
        //{
        //    var book = await _service.GetByIdAsync(id);
        //    if (book == null)
        //        return NotFound();

        //    return Ok(book);
        //}

        // POST: api/books
        [HttpPost(Name = "AddBook")]
        public async Task<ActionResult<ResponseResult>> Post([FromBody] CreateBookDto book)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return  BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var newBook = await service.AddBook(book);
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

        //// PUT: api/books/5
        //[HttpPut("{id}")]
        //public async Task<ActionResult> Put(int id, [FromBody] CreateBookDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updated = await _service.UpdateAsync(id, dto);
        //    if (!updated)
        //        return NotFound();

        //    return NoContent();
        //}

        //// DELETE: api/books/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var deleted = await _service.DeleteAsync(id);
        //    if (!deleted)
        //        return NotFound();

        //    return NoContent();
        //}
    }
}
