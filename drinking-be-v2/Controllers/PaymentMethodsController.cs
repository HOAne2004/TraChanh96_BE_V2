using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Interfaces.OrderInterfaces; // Chứa IPaymentMethodService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentService;

        public PaymentMethodsController(IPaymentMethodService paymentService)
        {
            _paymentService = paymentService;
        }

        // 1. GET ALL (ADMIN)
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllMethodsAsync();
            return Ok(result);
        }

        // 2. GET ACTIVE (CLIENT)
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive()
        {
            var result = await _paymentService.GetActiveMethodsAsync();
            return Ok(result);
        }

        // 3. GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            if (result == null) return NotFound("Phương thức thanh toán không tồn tại");
            return Ok(result);
        }

        // 4. CREATE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PaymentMethodCreateDto dto)
        {
            var result = await _paymentService.CreateAsync(dto);
            return Ok(result);
        }

        // 5. UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentMethodUpdateDto dto)
        {
            var result = await _paymentService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 6. TOGGLE STATUS (BẬT/TẮT)
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _paymentService.ToggleStatusAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        // 7. DELETE (SOFT DELETE)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _paymentService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Đã xóa phương thức thanh toán" });
        }
    }
}