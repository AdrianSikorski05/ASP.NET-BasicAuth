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
        /// Get user by id.
        /// </summary>
        /// <param name="getUserById"></param>
        /// <returns>Return response object with searched user..</returns>
        /// <response code="200">Return user</response>
        /// <response code="404">Dont found user</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> GetUserById(GetUserById getUserById)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }
            var result = await mediator.Send(new GetUserByIdQuery(getUserById.Id));
            if (result == null)
            {
                logger.LogError($"User with id {getUserById.Id} not found.");
                return NotFound(ResponseResult.NotFound($"User with id {getUserById.Id} not found."));
            }
            else
            {
                logger.LogInformation($"User with id {result.Id} found.");
                return Ok(ResponseResult.Success("OK", result));
            }
        }

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user">New user</param>
        /// <returns>Return a new user</returns>
        /// <response code="200">Return new user</response>
        /// <response code="404">Dont found user</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="updateUserDto">User who will updating</param>
        /// <returns>Object of response with user</returns>
        /// <response code="200">Return updated user</response>
        /// <response code="404">Dont found user</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }
            var result = await mediator.Send(new UpdateUserCommand(updateUserDto));
            if (result.Item2 == 0)
            {
                logger.LogError($"User with id {updateUserDto.Id} not found.");
                return NotFound(ResponseResult.NotFound($"User with id {updateUserDto.Id} not found."));
            }
            else
            {
                logger.LogInformation($"User with id {result.Item1.Id} updated.");
                return Ok(ResponseResult.Success("OK", result.Item1));
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="deleteUserDto">User who will delete</param>
        /// <returns></returns>
        /// <response code="200">Sucess</response>
        /// <response code="404">Dont found user</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpDelete(Name = "DeleteUser")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> DeleteUser([FromBody] DeleteUserDto deleteUserDto) 
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }

            var result = await mediator.Send(new DeleteUserCommand(deleteUserDto));
            if (result == 0)
            {
                logger.LogError($"User with id {deleteUserDto.Id} not found.");
                return NotFound(ResponseResult.NotFound($"User with id {deleteUserDto.Id} not found."));
            }
            else
            {
                logger.LogInformation($"User with id {deleteUserDto.Id} deleted.");
                return Ok(ResponseResult.Success("OK", new { Id = deleteUserDto.Id }));
            }
        }

        /// <summary>
        /// Delete user by id.
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        /// <response code="200">Sucess</response>
        /// <response code="404">Dont found user</response> 
        /// <response code="400">Invalid request parameters</response>
        [HttpDelete("{id}",Name = "DeleteUserById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        [ProducesResponseType(typeof(ResponseResult), 404)]
        [ProducesResponseType(typeof(ResponseResult), 400)]
        public async Task<ActionResult<ResponseResult>> DeleteUserById([FromRoute] DeleteUserByIdDto deleteUserByIdDto) 
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }
            var result = await mediator.Send(new DeleteUserByIdCommand(deleteUserByIdDto));
            if (result == 0)
            {
                logger.LogError($"User with id {deleteUserByIdDto.Id} not found.");
                return NotFound(ResponseResult.NotFound($"User with id {deleteUserByIdDto.Id} not found."));
            }
            else
            {
                logger.LogInformation($"User with id {deleteUserByIdDto.Id} deleted.");
                return Ok(ResponseResult.Success("OK", $"User with id {deleteUserByIdDto.Id} deleted."));
            }
        }
    }
}
