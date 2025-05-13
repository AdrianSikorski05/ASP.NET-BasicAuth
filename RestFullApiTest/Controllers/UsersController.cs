using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [Authorize(AuthenticationSchemes = "BasicAthentication")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
    {
        // GET: /UsersController
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<IEnumerable<User?>>> GetAllUsers()
        {
            try
            {
                var users = await userService.GetAllUsers();
                if (users == null)
                { 
                    logger.LogError("Users not founds.");
                    return NotFound("Users not founds.");
                }
                else
                {
                    logger.LogInformation($"Founds {users.ToList().Count} users.");
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting users");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST /UsersController
        [HttpPost(Name = "AddUser")]
        public async Task<ActionResult> Post([FromBody] CreateUserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                { 
                    logger.LogError("Model state is invalid");
                    return BadRequest(ModelState);
                }

                var newUser = await userService.AddUser(user);
                if (newUser == null)
                {
                    logger.LogError("User not created");
                    return BadRequest("User not created");
                }
                else
                {
                    logger.LogInformation($"User {newUser.Username} created.");
                    return CreatedAtAction(nameof(Post), new { newUser }, null);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating user");
                return StatusCode(500, "Internal server error");
            }
        }
      
    }
}
