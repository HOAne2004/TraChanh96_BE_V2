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
                // "sub": Dùng PublicId (GUID) để định danh an toàn trên API
                new Claim(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()), 
                
                // "jti": ID duy nhất của Token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                // "nameid": ID nội bộ (Int) - dùng cho các query nội bộ nhanh gọn
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                
                // "unique_name": Username
                new Claim(ClaimTypes.Name, user.Username),
                
                // "role": Vai trò (Admin, Manager, Staff, Customer)
                new Claim(ClaimTypes.Role, user.RoleId.ToString()), 
                
                // Email
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 2. ⭐ BỔ SUNG: Thông tin Nhân sự & Cửa hàng
            // Lưu ý quan trọng: Tại AuthService khi gọi hàm này, User bắt buộc phải được .Include(u => u.Staff)
            if (user.Staff != null)
            {
                // Thêm StaffId để tiện cho các logic chấm công, tính lương
                claims.Add(new Claim("StaffId", user.Staff.Id.ToString()));

                // Thêm StaffPublicId
                claims.Add(new Claim("StaffPublicId", user.Staff.PublicId.ToString()));

                // Thêm StoreId: Nếu nhân viên thuộc cửa hàng cụ thể (Manager/Barista)
                // Frontend sẽ dùng cái này để filter dữ liệu chỉ của cửa hàng đó
                if (user.Staff.StoreId.HasValue)
                {
                    claims.Add(new Claim("StoreId", user.Staff.StoreId.Value.ToString()));
                }
            }

            // 3. Tạo Credentials (Ký tên)
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            // 4. Cấu hình Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Thời hạn 7 ngày (Có thể cấu hình trong appsettings nếu muốn)
                Expires = DateTime.UtcNow.AddDays(7),
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