using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodService _methodService;

        public PaymentMethodsController(IPaymentMethodService methodService)
        {
            _methodService = methodService;
        }

        // --- PUBLIC API ---

        [HttpGet("active")]
        [AllowAnonymous] // Khách vãng lai cũng cần xem để chọn thanh toán
        public async Task<IActionResult> GetActiveMethods()
        {
            var methods = await _methodService.GetActiveMethodsAsync();
            return Ok(methods);
        }

        // --- ADMIN API ---

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllMethods()
        {
            var methods = await _methodService.GetAllMethodsAsync();
            return Ok(methods);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var method = await _methodService.GetByIdAsync(id);
            if (method == null) return NotFound();
            return Ok(method);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PaymentMethodCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _methodService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentMethodUpdateDto dto)
        {
            var result = await _methodService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _methodService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}