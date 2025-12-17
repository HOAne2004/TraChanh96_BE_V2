using drinking_be.Dtos.InventoryDtos;
using drinking_be.Interfaces.ProductInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ quản lý mới được xem kho
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/inventory?storeId=1&search=Cafe
        // Nếu không truyền storeId -> Mặc định lấy Kho Tổng
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] string? search)
        {
            var result = await _inventoryService.GetAllAsync(storeId, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _inventoryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/inventory (Khởi tạo tồn kho đầu kỳ)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _inventoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/inventory/{id} (Cập nhật số lượng/Kiểm kho)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] InventoryUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _inventoryService.UpdateQuantityAsync(id, dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _inventoryService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}