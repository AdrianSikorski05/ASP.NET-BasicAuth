using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestFullApiTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost(Name = "Login")]
        public async Task<ActionResult<ResponseResult>> Login([FromBody] LoginDto dto, [FromServices] IUserRepository repo, [FromServices] JwtService jwt)
        {
            var user = await repo.GetUserByUsername(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized(ResponseResult.Unauthorized("Invalid username or password"));

            var token = jwt.GenerateToken(user.Username);
            return Ok(ResponseResult.Success("OK", new { token }));
        }

        [Authorize]
        [HttpGet("secure")]
        public async Task<ActionResult<ResponseResult>> Secure()
        {
            var username = User.Identity?.Name;
            return Ok(ResponseResult.Success("OK", new { message = $"Hello {username}, this is a secure endpoint!" }));
        }
    }
}
