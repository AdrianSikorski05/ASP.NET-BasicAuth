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
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromServices] IUserRepository repo, [FromServices] JwtService jwt)
        {
            var user = await repo.GetUserByUsername(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid credentials");

            var token = jwt.GenerateToken(user.Username);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("secure")]
        public IActionResult Secure()
        {
            var username = User.Identity?.Name;
            return Ok($"Cześć, {username}! Masz dostęp.");
        }
    }
}
