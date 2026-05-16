using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chốt quyền chặt chẽ
    public class SettingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingService _settingService;

        public SettingsController(IUnitOfWork unitOfWork, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
        }

        // 1. LẤY DANH SÁCH CẤU HÌNH (Đổ ra giao diện Admin)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSettings()
        {
            var settings = await _unitOfWork.Repository<SystemSetting>().GetAllAsync();
            return Ok(new { Success = true, Data = settings });
        }

        // 2. CẬP NHẬT 1 CẤU HÌNH (Gọi khi Admin bấm "Lưu")
        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Value))
            {
                return BadRequest(new { Success = false, Message = "Giá trị không được để trống." });
            }

            try
            {
                await _settingService.UpdateSettingAsync(key, dto.Value);
                return Ok(new { Success = true, Message = "Cập nhật cấu hình thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }

    // DTO siêu gọn nhẹ
    public class UpdateSettingDto
    {
        public string Value { get; set; } = null!;
    }
}