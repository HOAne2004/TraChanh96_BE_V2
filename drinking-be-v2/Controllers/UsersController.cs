using drinking_be.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Dtos.Common;

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
        protected Guid GetUserPublicId()
        {
            // 1. Chưa được xác thực
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng chưa được xác thực.");
            }

            // 2. Ưu tiên chuẩn JWT: sub
            var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            // 3. Fallback: NameIdentifier (phòng khi BE đổi mapping)
            subClaim ??= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(subClaim, out var publicId))
            {
                return publicId;
            }

            throw new UnauthorizedAccessException("Token không chứa PublicId hợp lệ.");
        }

        // Get all
        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        {
            var result = await _userService.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Xem chi tiết người dùng theo PublicId
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Manager")] // Chỉ Admin/Manager được xem
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByPublicIdAsync(id);

            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });
            return Ok(user);
        }

        /// <summary>
        /// Xem hồ sơ cá nhân (Profile)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
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
        /// [ADMIN] Cập nhật thông tin người dùng (Status, Role,...)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")] // Chỉ Admin được sửa người khác
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto updateDto)
        {
            // Gọi service update (Cần đảm bảo hàm UpdateUserByPublicIdAsync cho phép sửa Status/Role)
            // Nếu UserUpdateDto của bạn chưa có Status, hãy tạo UserAdminUpdateDto riêng hoặc bổ sung vào.

            var updatedUser = await _userService.UpdateUserByPublicIdAsync(id, updateDto);

            if (updatedUser == null) return NotFound(new { message = "Không tìm thấy người dùng." });
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