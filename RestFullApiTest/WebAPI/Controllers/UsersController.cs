using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    [Tags("Users")]
    public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
    {
        // GET: /UsersController
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<ResponseResult>> GetAllUsers()
        {
            var users = await userService.GetAllUsers();
            if (users == null)
            {
                logger.LogError("Users not founds.");
                return NotFound(ResponseResult.NotFound("Users not founds."));
            }
            else
            {
                logger.LogInformation($"Founds {users.ToList().Count} users.");
                return Ok(ResponseResult.Success("OK", users));
            }
        }

        // POST /UsersController
        [HttpPost(Name = "AddUser")]
        public async Task<ActionResult<ResponseResult>> Post([FromBody] CreateUserDto user)
        {

            if (!ModelState.IsValid)
            {
                logger.LogError("Model state is invalid");
                return BadRequest(ResponseResult.BadRequest("Model state is invalid", ModelState));
            }

            var newUser = await userService.AddUser(user);
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
