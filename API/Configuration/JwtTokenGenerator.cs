using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNI_Events.API.Configuration
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(long userId, string name, string email, string role);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JWTSettings _settings;

        public JwtTokenGenerator(IOptions<JWTSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(long userId, string name, string email, string role)
        {
            var key = Encoding.ASCII.GetBytes(_settings.Secret);
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.UniqueName, name),
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
