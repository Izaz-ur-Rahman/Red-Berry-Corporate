using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedBerryCorporate.Configuration;
using RedBerryCorporate.DTOs.Auth;
using RedBerryCorporate.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedBerryCorporate.Helpers
{
    public class JwtHelper
    {
        private readonly JwtSettings _settings;

        public JwtHelper(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public JwtTokenResponse GenerateToken(User user, string employeeName = "")
        {
            var expiry = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim("EmpId", user.EmpId?.ToString() ?? ""),
        new Claim(ClaimTypes.Role, user.RoleNames ?? "User"),
        new Claim("EmployeeName", employeeName)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtTokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiry
            };
        }
    }
}