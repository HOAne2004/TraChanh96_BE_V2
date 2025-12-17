// Controllers/AdminController.cs
using drinking_be.Dtos.UserDtos;
using drinking_be.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // --- 1. GET ALL USERS (/api/Admin/users) ---
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserReadDto>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        // --- 2. UPDATE USER ROLE/STATUS (PATCH /api/Admin/users/{publicId}) ---
        /// <summary>
        /// [ADMIN] Cập nhật vai trò, trạng thái hoặc thông tin cá nhân của người dùng.
        /// </summary>
        [HttpPatch("users/{publicId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(Guid publicId, [FromBody] UserUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedUser = await _adminService.UpdateUserByPublicIdAsync(publicId, updateDto);
                if (updatedUser == null) return NotFound("Không tìm thấy người dùng.");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- 3. DELETE USER (DELETE /api/Admin/users/{publicId}) ---
        /// <summary>
        /// [ADMIN] Xóa hoặc vô hiệu hóa tài khoản người dùng.
        /// </summary>
        [HttpDelete("users/{publicId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid publicId)
        {
            var result = await _adminService.DeleteUserByPublicIdAsync(publicId);
            if (!result) return NotFound("Không tìm thấy người dùng.");

            return NoContent(); // 204 No Content
        }
    }
}