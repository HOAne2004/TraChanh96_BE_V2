using drinking_be.Dtos.VoucherDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherTemplatesController : ControllerBase
    {
        private readonly IVoucherTemplateService _templateService;

        public VoucherTemplatesController(IVoucherTemplateService templateService)
        {
            _templateService = templateService;
        }

        // GET: api/vouchertemplates?status=Active
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] PublicStatusEnum? status)
        {
            var result = await _templateService.GetAllAsync(search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _templateService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // --- ADMIN ONLY ---

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] VoucherTemplateCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _templateService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] VoucherTemplateUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _templateService.UpdateAsync(id, dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _templateService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}