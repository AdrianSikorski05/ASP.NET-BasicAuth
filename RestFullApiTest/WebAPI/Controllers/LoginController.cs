﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestFullApiTest.Application.RefreshToken.Dtos;
using System.Security.Claims;


namespace RestFullApiTest
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Authentication")]
    public class LoginController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ResponseResult>> Login([FromBody] LoginDto dto, [FromServices] IUserRepository repo, [FromServices] IRefreshTokenRepository refreshTokenRepo, [FromServices] JwtService jwt, ILogger<LoginController> logger)
        {
            var user = await repo.GetUserByUsername(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized(ResponseResult.Unauthorized("Invalid username or password"));

            var token = jwt.GenerateToken(user);
            var refreshToken = jwt.GenerateRefreshToken();

            await refreshTokenRepo.SaveToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(1));
            logger.LogInformation(@$"Token taken by {user.Username}");
            return Ok(ResponseResult.Success("OK", new { token = token, refreshToken = refreshToken }));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ResponseResult>> RefreshToken([FromBody] RefreshRequestDto dto, [FromServices] IRefreshTokenRepository refreshTokenRepo, [FromServices] IUserRepository repo, [FromServices] JwtService jwt)
        { 
            var token = await refreshTokenRepo.Get(dto.RefreshToken);
            if (token == null || token.Revoked || token.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(ResponseResult.Unauthorized("Invalid or expired refresh token"));

            var user = await repo.GetUserById(token.UserId);
            if (user == null)
                return Unauthorized(ResponseResult.Unauthorized("User not found"));

            await refreshTokenRepo.Revoke(token.Id);

            var newAccessToken = jwt.GenerateToken(user);
            var newRefreshToken = jwt.GenerateRefreshToken();
            await refreshTokenRepo.SaveToken(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(1));

            return Ok(ResponseResult.Success("Token refreshed", new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken
            }));

        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var user = new User
            {
                Id = Convert.ToInt32(id),
                Username = username!,
                Role = role!
            };

            return Ok(user);
        }

        [Authorize]
        [HttpGet("secure")]
        public async Task<ActionResult<ResponseResult>> Secure()
        {
            var username = User.Identity?.Name;
            return Ok(ResponseResult.Success("OK", new { message = $"Hello {username}, this is a secure endpoint!" }));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-data")]
        public IActionResult GetAdminData()
        {
            return Ok(ResponseResult.Success("Tylko dla admina"));
        }
    }
}
