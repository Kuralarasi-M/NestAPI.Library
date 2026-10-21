using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibraryApp.Feature.Auth.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var keyString = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var durationMinutes = double.TryParse(jwtSection["DurationInMinutes"], out var duration) ? duration : 1440;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
