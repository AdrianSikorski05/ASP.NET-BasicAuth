using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest
{
    /// <summary>
    /// Operation with comments.
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [Tags("Comments")]
    public class CommentController(ILogger<BooksController> logger, IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Returns comments for a given book, pagination and sorting.
        /// </summary>
        /// <param name="filter">Filters</param>
        /// <returns>List of comment for book</returns>
        /// <response code="200">Return List of comments</response>
        /// <response code="404">Dont found comments</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpGet("comments")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetComments([FromQuery] CommentFilter filter)
        {
            var result = await mediator.Send(new GetCommentsQuery(filter));
            if (!result.Items.Any())
            {
                logger.LogInformation("Books not founds.");
                return NotFound(ResponseResult.NotFound("Comments not founds."));
            }
            else
            {
                logger.LogInformation($"Founds {result.Items.ToList().Count} comments");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Returns new comment for a given book.
        /// </summary>
        /// <param name="createCommentBookDto">newComment</param>
        /// <returns>New comment</returns>
        /// <response code="200">Return new comment</response>
        /// <response code="404">Dont found comment</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPost("addComment")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> AddComment([FromBody] CreateCommentBookDto createCommentBookDto) 
        {

            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var newComment = await mediator.Send(new CreateCommentCommand(createCommentBookDto));
            if (newComment == null)
            {
                logger.LogError("Comment not created");
                return BadRequest(ResponseResult.BadRequest("Comment not created"));
            }
            else
            {
                logger.LogInformation($"Comment {newComment.id} created");
                return Ok(ResponseResult.Success("Comment created", newComment));
            }
        }


        /// <summary>
        /// Returns updated comment for a given book.
        /// </summary>
        /// <param name="updateCommentBookDto">Comment who will  updating.</param>
        /// <returns>Updated comment</returns>
        /// <response code="200">Return updated comment</response>
        /// <response code="404">Dont found comment</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPost("updateComment")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> UpdateComment([FromBody] UpdateCommentBookDto updateCommentBookDto)
        {

            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is not valid");
                return BadRequest(ResponseResult.BadRequest("Model state is not valid", ModelState));
            }

            var uComment = await mediator.Send(new UpdateCommentCommand(updateCommentBookDto));
            if (uComment == null)
            {
                logger.LogError("Comment not updated");
                return NotFound(ResponseResult.NotFound("Comment not updated"));
            }
            else
            {
                logger.LogInformation($"Comment {uComment.id} updated");
                return Ok(ResponseResult.Success("Comment updated", uComment));
            }
        }

        /// <summary>
        /// Delete comment by id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Response true/false</returns>
        [HttpDelete("deleteComment/{id:int}")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> DeleteComment(int id)
        {
            if (id <= 0) 
            {
                logger.LogError("Parameter is null or empty.");
                return BadRequest(ResponseResult.BadRequest("Parameter is null or empty.", id));
            }

            var result = await mediator.Send(new DeleteCommentByIdCommand(id));
            if (!result)
            {
                logger.LogError($"Comment with id {id} not found.");
                return NotFound(ResponseResult.NotFound($"Comment with id {id} not found."));
            }
            else
            {
                logger.LogInformation($"Comment with id {id} deleted.");
                return Ok(ResponseResult.Success("Comment deleted", result));
            }
        }
    }
}
