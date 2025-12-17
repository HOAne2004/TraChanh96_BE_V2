using drinking_be.Dtos.AttendanceDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // Helper lấy StaffId từ Token
        private int GetCurrentStaffId()
        {
            // Token phải có Claim "StaffId" (đã cấu hình trong JwtGenerator)
            var staffIdClaim = User.FindFirst("StaffId");
            if (staffIdClaim == null || !int.TryParse(staffIdClaim.Value, out int staffId))
            {
                throw new UnauthorizedAccessException("Tài khoản này không phải là Nhân viên (Không có Staff Profile).");
            }
            return staffId;
        }

        // --- NHÂN VIÊN TỰ CHẤM ---

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                var staffId = GetCurrentStaffId();
                var result = await _attendanceService.CheckInAsync(staffId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var staffId = GetCurrentStaffId();
                var result = await _attendanceService.CheckOutAsync(staffId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("me/today")]
        public async Task<IActionResult> GetMyToday()
        {
            try
            {
                var staffId = GetCurrentStaffId();
                var result = await _attendanceService.GetTodayAttendanceAsync(staffId);
                if (result == null) return NotFound("Hôm nay chưa chấm công.");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        }

        [HttpGet("me/history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var staffId = GetCurrentStaffId();
                if (month == 0) month = DateTime.Now.Month;
                if (year == 0) year = DateTime.Now.Year;

                var result = await _attendanceService.GetStaffHistoryAsync(staffId, month, year);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        }

        // --- MANAGER QUẢN LÝ ---

        [HttpGet("store/{storeId}/report")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStoreReport(int storeId, [FromQuery] DateTime? date)
        {
            // Mặc định lấy hôm nay nếu không truyền date
            var queryDate = date.HasValue ? DateOnly.FromDateTime(date.Value) : DateOnly.FromDateTime(DateTime.UtcNow);

            var result = await _attendanceService.GetStoreDailyReportAsync(storeId, queryDate);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(long id, [FromBody] AttendanceUpdateDto dto)
        {
            var result = await _attendanceService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("manual")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateManual([FromBody] AttendanceCreateDto dto)
        {
            try
            {
                var result = await _attendanceService.CreateManualAsync(dto);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}