using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestFullApiTest.Logic.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [Authorize(AuthenticationSchemes = "BasicAthentication")]
    [Route("[controller]")]
    [ApiController]
    public class BooksController(IBookService service, ILogger<BooksController> logger) : ControllerBase
    {

        /// <summary>
        /// Pobiera wszystkie książki
        /// </summary>
        /// <returns>zwraca obiekt Ok</returns>
        [HttpGet(Name = "Books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> Get()
        {
            try
            {
                var books = await service.GetAllAsync();
                if (books == null)
                { 
                    logger.LogInformation("Books not founds.");
                    return NotFound("Books not founds.");
                }
                else
                {
                    logger.LogInformation($"Founds {books.ToList().Count} books");
                    return Ok(books);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting books");
                return StatusCode(500, "Internal server error");
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
        public async Task<ActionResult> Post([FromBody] CreateBookDto book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogError("Model state is not valid");
                    return BadRequest(ModelState);
                }

                var newBook = await service.AddBook(book);
                if (newBook == null)
                {
                    logger.LogError("Book not created");
                    return BadRequest("Book not created");
                }
                else
                {
                    logger.LogInformation($"Book {newBook.Title} created");
                    return CreatedAtAction(nameof(Get), new { newBook }, null);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding book");
                return StatusCode(500, "Internal server error");
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
