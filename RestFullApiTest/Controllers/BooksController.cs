using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestFullApiTest.Logic.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [Authorize(AuthenticationSchemes = "BasicAthentication")]
    [Route("[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        /// <summary>
        /// Pobiera wszystkie książki
        /// </summary>
        /// <returns>zwraca obiekt Ok</returns>
        [HttpGet(Name ="Books")]
        public async Task<ActionResult<IEnumerable<BookDto>>> Get()
        {
            var books = await _service.GetAllAsync();
            return Ok(books);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AddBook(book);
            return CreatedAtAction(nameof(Get), new { id }, null);
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
