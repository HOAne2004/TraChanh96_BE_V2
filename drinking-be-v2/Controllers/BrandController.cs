using drinking_be.Dtos.BrandDtos;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // --- HELPER ---
        private BrandReadDto GetDefaultBrandInfo()
        {
            return new BrandReadDto
            {
                Name = "Trà Chanh 96 (Mặc định)",
                CompanyName = "Công ty TNHH Trà Chanh 96",
                LogoUrl = "https://placehold.co/100x100?text=Logo",
                Address = "Vui lòng cập nhật thông tin trong trang quản trị",
                Hotline = "1900 xxxx",
                EmailSupport = "support@trachanh96.vn",
                Status = "Active"
            };
        }

        // --- PUBLIC API (Dùng cho cả Footer, AppConfig, Contact...) ---

        [HttpGet("info")] // Đặt route là /api/brand/info
        [AllowAnonymous]  // Khách không cần login cũng xem được
        public async Task<IActionResult> GetPublicBrandInfo()
        {
            var brand = await _brandService.GetPrimaryBrandInfoAsync();
            // Luôn trả về dữ liệu (Thật hoặc Mặc định) để FE không bị crash
            return Ok(brand ?? GetDefaultBrandInfo());
        }

        // --- ADMIN API (Quản lý) ---

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] BrandCreateDto brandDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _brandService.CreateBrandAsync(brandDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDto brandDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _brandService.UpdateBrandAsync(id, brandDto);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
