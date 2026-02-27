using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace drinking_be.Utils
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public JwtGenerator(IConfiguration config)
        {
            _config = config;
            var secret = _config["JwtSettings:Key"];

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("JwtSettings:Key không được cấu hình trong appsettings.json.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string CreateToken(User user)
        {
            // 1. Khởi tạo danh sách Claims cơ bản
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()), 
                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                new Claim("UserId", user.Id.ToString()),
                
                new Claim(ClaimTypes.Name, user.Username),
                
                new Claim(ClaimTypes.Role, user.RoleId.ToString()), 
                
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (user.Staff != null)
            {
                claims.Add(new Claim("StaffId", user.Staff.Id.ToString()));

                claims.Add(new Claim("StaffPublicId", user.Staff.PublicId.ToString()));

                if (user.Staff.StoreId.HasValue)
                {
                    claims.Add(new Claim("StoreId", user.Staff.StoreId.Value.ToString()));
                }
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(360),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}