using drinking_be.Dtos.PayslipDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ quản lý mới được tính lương
    public class PayslipsController : ControllerBase
    {
        private readonly IPayslipService _payslipService;

        public PayslipsController(IPayslipService payslipService)
        {
            _payslipService = payslipService;
        }

        // POST: api/payslips/calculate
        // Tính toán và tạo phiếu lương cho 1 nhân viên
        [HttpPost("calculate")]
        public async Task<IActionResult> Generate([FromBody] PayslipCreateDto createDto)
        {
            try
            {
                var result = await _payslipService.GeneratePayslipAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/payslips?month=12&year=2025
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var result = await _payslipService.GetAllAsync(storeId, month, year);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _payslipService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // PUT: api/payslips/{id} -> Update thưởng phạt / Chốt lương
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] PayslipUpdateDto updateDto)
        {
            try
            {
                var result = await _payslipService.UpdateAsync(id, updateDto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _payslipService.DeleteAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}