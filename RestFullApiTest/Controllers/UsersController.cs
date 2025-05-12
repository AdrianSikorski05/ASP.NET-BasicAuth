using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestFullApiTest
{
    [Authorize(AuthenticationSchemes = "BasicAthentication")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /UsersController
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<IEnumerable<User?>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            if (users == null)
                return NotFound("Users not founds.");

            return Ok(users);
        }

        // POST /UsersController
        [HttpPost(Name = "AddUser")]
        public async Task<ActionResult> Post([FromBody] CreateUserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _userService.AddUser(user);
            return CreatedAtAction(nameof(Post), new { id }, null);
        }
      
    }
}
