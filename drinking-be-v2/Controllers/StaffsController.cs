using drinking_be.Dtos.StaffDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ quản lý mới được vào
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: api/staffs?storeId=1&search=Huy
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] string? search)
        {
            var staffs = await _staffService.GetAllAsync(storeId, search);
            return Ok(staffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffCreateDto createDto)
        {
            try
            {
                var newStaff = await _staffService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = newStaff.Id }, newStaff);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffUpdateDto updateDto)
        {
            // Kiểm tra bảo mật chéo cấp
            var targetStaff = await _staffService.GetByIdAsync(id);
            if (targetStaff == null) return NotFound();

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Nếu người đang sửa chỉ là Manager, nhưng lại đi sửa hồ sơ của một StoreManager khác -> Cấm.
            if (currentUserRole == "Manager" && targetStaff.Position == "Quản lý cửa hàng") // Hoặc check Enum thay vì chuỗi
            {
                return StatusCode(403, "Bạn không có quyền chỉnh sửa hồ sơ của một Quản lý khác. Vui lòng liên hệ Admin.");
            }

            var updatedStaff = await _staffService.UpdateAsync(id, updateDto);
            return Ok(updatedStaff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var targetStaff = await _staffService.GetByIdAsync(id);
            if (targetStaff == null) return NotFound();

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole == "Manager" && targetStaff.Position == "Quản lý cửa hàng")
            {
                return StatusCode(403, "Bạn không có quyền xóa (đuổi việc) một Quản lý khác. Vui lòng liên hệ Admin.");
            }

            var result = await _staffService.DeleteAsync(id);
            if (!result) return BadRequest("Có lỗi xảy ra khi xóa.");
            return NoContent();
        }
    }
}