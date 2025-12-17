using drinking_be.Dtos.SocialMediaDtos;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediasController : ControllerBase
    {
        private readonly ISocialMediaService _socialService;

        public SocialMediasController(ISocialMediaService socialService)
        {
            _socialService = socialService;
        }

        // GET: api/socialmedias/active?brandId=1
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive([FromQuery] int brandId = 1, [FromQuery] int? storeId = null)
        {
            var result = await _socialService.GetActiveSocialsAsync(brandId, storeId);
            return Ok(result);
        }

        // GET: api/socialmedias/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] int? brandId, [FromQuery] int? storeId)
        {
            var result = await _socialService.GetAllAsync(brandId, storeId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _socialService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] SocialMediaCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _socialService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] SocialMediaUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _socialService.UpdateAsync(id, dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _socialService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}