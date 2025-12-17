using drinking_be.Dtos.BannerDtos;
using drinking_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? position)
        {
            var banners = await _bannerService.GetActiveBannersAsync(position);
            return Ok(banners);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")] // Uncomment khi cần bảo mật
        public async Task<IActionResult> Create([FromBody] BannerCreateDto dto)
        {
            var result = await _bannerService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bannerService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}