using drinking_be.Dtos.FranchiseDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FranchiseController : ControllerBase
    {
        private readonly IFranchiseService _franchiseService;

        public FranchiseController(IFranchiseService franchiseService)
        {
            _franchiseService = franchiseService;
        }

        // --- PUBLIC ENDPOINT ---

        /// <summary>
        /// Gửi yêu cầu nhượng quyền (Không cần đăng nhập).
        /// </summary>
        [HttpPost]
        [AllowAnonymous] // ⭐️ Quan trọng: Để khách vãng lai gửi được
        public async Task<IActionResult> Create([FromBody] FranchiseCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _franchiseService.CreateRequestAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- ADMIN/MANAGER ENDPOINTS ---

        /// <summary>
        /// [Admin] Lấy danh sách yêu cầu. Có thể lọc theo Status.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] FranchiseStatusEnum? status, [FromQuery] string? search)
        {
            var result = await _franchiseService.GetAllAsync(status, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _franchiseService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] FranchiseUpdateDto dto)
        {
            var result = await _franchiseService.UpdateRequestAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin cao nhất mới được xóa
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _franchiseService.DeleteRequestAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}