using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SHDomain.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SHDomain.Helpers
{
    public class TokenHelper(IConfiguration config)
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly SymmetricSecurityKey SIGNING_KEY = new(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

        public string GetJwtToken(User user)
        {
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var credentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);

            int expirationInMinutes = int.Parse(config["Jwt:ExpirationInMinutes"] ?? "10");

            var description = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = credentials
            };

            var token = _tokenHandler.CreateToken(description);

            return _tokenHandler.WriteToken(token);
        }
    }
}
