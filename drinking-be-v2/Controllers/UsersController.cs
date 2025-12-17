using drinking_be.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using drinking_be.Interfaces.AuthInterfaces;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 Bắt buộc đăng nhập cho tất cả các hàm bên dưới
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Helper function để lấy PublicId từ Token
        private Guid GetUserPublicId()
        {
            var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (Guid.TryParse(subClaim, out var publicId))
            {
                return publicId;
            }
            throw new UnauthorizedAccessException("Token không hợp lệ.");
        }

        /// <summary>
        /// Xem hồ sơ cá nhân (Profile)
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var publicId = GetUserPublicId();
            var user = await _userService.GetUserByPublicIdAsync(publicId);

            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Cập nhật hồ sơ cá nhân
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDto updateDto)
        {
            var publicId = GetUserPublicId();

            // Ngăn chặn User tự sửa Role hoặc Status thông qua API này
            // (DTO đã xử lý 1 phần, nhưng ở đây có thể check thêm nếu cần)

            var updatedUser = await _userService.UpdateUserByPublicIdAsync(publicId, updateDto);

            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        /// <summary>
        /// Xóa tài khoản (Tự xóa)
        /// </summary>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var publicId = GetUserPublicId();
            var result = await _userService.DeleteUserByPublicIdAsync(publicId);

            if (!result) return NotFound();
            return NoContent();
        }
    }
}