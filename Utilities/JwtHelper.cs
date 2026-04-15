using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SchoolERP.Net.Utilities
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT for the authenticated user.
        /// Claims aligned with TDD 12.7 UserSessionModel.
        /// </summary>
        public string GenerateToken(string username, string roleName, int userId, int userTypeId, int defaultRoleId)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SchoolERP_Default_Key_1234567890"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,       username),
                new Claim(ClaimTypes.Role,       roleName),
                new Claim("UserId",              userId.ToString()),
                new Claim("UserTypeId",          userTypeId.ToString()),
                new Claim("DefaultRoleId",       defaultRoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer:            _configuration["Jwt:Issuer"],
                audience:          _configuration["Jwt:Audience"],
                claims:            claims,
                expires:           DateTime.UtcNow.AddMinutes(
                                       Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
