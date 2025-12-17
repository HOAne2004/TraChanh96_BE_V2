using drinking_be.Dtos.SupplyOrderDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ nội bộ mới được dùng
    public class SupplyOrdersController : ControllerBase
    {
        private readonly ISupplyOrderService _supplyOrderService;

        public SupplyOrdersController(ISupplyOrderService supplyOrderService)
        {
            _supplyOrderService = supplyOrderService;
        }

        // Helper GetUserId
        private int GetUserId()
        {
            var claim = User.FindFirst("nameid") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId)) return userId;
            throw new UnauthorizedAccessException("Token không hợp lệ.");
        }

        // POST: api/supplyorders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplyOrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _supplyOrderService.CreateAsync(GetUserId(), dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/supplyorders
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? storeId,
            [FromQuery] SupplyOrderStatusEnum? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var result = await _supplyOrderService.GetAllAsync(storeId, status, from, to);
            return Ok(result);
        }

        // GET: api/supplyorders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _supplyOrderService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // PUT: api/supplyorders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] SupplyOrderUpdateDto dto)
        {
            try
            {
                var result = await _supplyOrderService.UpdateAsync(id, GetUserId(), dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}