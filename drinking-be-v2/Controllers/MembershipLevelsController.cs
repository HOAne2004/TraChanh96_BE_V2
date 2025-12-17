using drinking_be.Dtos.MembershipLevelDtos;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipLevelsController : ControllerBase
    {
        private readonly IMembershipLevelService _levelService;

        public MembershipLevelsController(IMembershipLevelService levelService)
        {
            _levelService = levelService;
        }

        // --- PUBLIC ENDPOINT (Khách hàng xem để biết quyền lợi) ---

        /// <summary>
        /// Lấy danh sách tất cả các cấp độ thành viên.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLevels()
        {
            var levels = await _levelService.GetAllLevelsAsync();
            return Ok(levels);
        }

        // --- ADMIN ENDPOINTS ---

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLevelById(byte id)
        {
            var level = await _levelService.GetByIdAsync(id);
            if (level == null) return NotFound();
            return Ok(level);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLevel([FromBody] MembershipLevelCreateDto levelDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdLevel = await _levelService.CreateLevelAsync(levelDto);
                return CreatedAtAction(nameof(GetLevelById), new { id = createdLevel.Id }, createdLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLevel(byte id, [FromBody] MembershipLevelUpdateDto levelDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedLevel = await _levelService.UpdateLevelAsync(id, levelDto);
            if (updatedLevel == null) return NotFound();

            return Ok(updatedLevel);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLevel(byte id)
        {
            var result = await _levelService.DeleteLevelAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}