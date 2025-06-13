using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace RestFullApiTest
{
    public class JwtService(IConfiguration config)
    {
        public string GenerateToken(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentNullException(nameof(user.Username));
            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentNullException(nameof(user.Role));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken() 
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}

