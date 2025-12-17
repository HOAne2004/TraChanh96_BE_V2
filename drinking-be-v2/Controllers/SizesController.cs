using drinking_be.Dtos.SizeDtos;
using drinking_be.Interfaces.OptionInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // GET: api/sizes (Mặc định lấy Active cho khách)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sizes = await _sizeService.GetAllAsync(activeOnly: true);
            return Ok(sizes);
        }

        // GET: api/sizes/admin (Lấy tất cả cho Admin quản lý)
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var sizes = await _sizeService.GetAllAsync(activeOnly: false);
            return Ok(sizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null) return NotFound();
            return Ok(size);
        }

        // ⭐️ API kiểm tra xem Size này đang dùng cho bao nhiêu sản phẩm
        [HttpGet("{id}/usage")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetSizeUsage(short id)
        {
            var count = await _sizeService.CountProductsUsingSizeAsync(id);
            return Ok(new { count });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] SizeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _sizeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(short id, [FromBody] SizeUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _sizeService.UpdateAsync(id, dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _sizeService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}