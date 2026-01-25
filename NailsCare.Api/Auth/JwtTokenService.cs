using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NailsCare.Api.Models;

namespace NailsCare.Api.Auth
{
    public class JwtTokenService
    {
        private readonly IConfiguration _cfg;

        public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

        public string CreateToken(User user)
        {
            var key = _cfg["Jwt:Key"]!;
            var issuer = _cfg["Jwt:Issuer"]!;
            var audience = _cfg["Jwt:Audience"]!;

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role),
            new("phone", user.Phone)
        };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}