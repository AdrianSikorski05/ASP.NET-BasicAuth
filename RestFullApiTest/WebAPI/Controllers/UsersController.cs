using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]/[action]")]
    [Tags("Users")]
    public class UsersController(ILogger<UsersController> logger, IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Return all users.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return List of users</response>
        /// <response code="404">Dont found users</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpGet(Name = "GetUsers")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetAllUsers([FromQuery] UserFilter userFilter)
        {
            var result = await mediator.Send(new GetAllUsersQuery(userFilter));
            if (result.Items.ToList().Count <= 0)
            {
                logger.LogError("Users not founds.");
                return NotFound(ResponseResult.NotFound("Users not founds."));
            }
            else
            {
                logger.LogInformation($"Founds {result.Items.ToList().Count} users.");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user">New user</param>
        /// <returns>Return a new user</returns>
        /// <response code="200">Return new user</response>
        /// <response code="404">Dont found users</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> AddNewUser([FromBody] CreateUserDto user)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }

            var newUser = await mediator.Send(new CreateUserCommand(user));
            if (newUser == null)
            {
                logger.LogError("User not created");
                return BadRequest(ResponseResult.BadRequest("User not created"));
            }
            else
            {
                logger.LogInformation($"User {newUser.Username} created.");
                return Ok(ResponseResult.Success("OK", newUser));
            }
        }
    }
}
