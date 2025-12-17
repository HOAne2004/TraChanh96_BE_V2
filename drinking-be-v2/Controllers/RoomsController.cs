using drinking_be.Dtos.RoomDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/rooms?storeId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId)
        {
            // Mặc định lấy tất cả (kể cả Inactive nếu là Admin xem danh sách quản lý)
            // Hoặc tách API riêng. Ở đây lấy activeOnly = true cho an toàn
            var result = await _roomService.GetAllAsync(storeId, activeOnly: true);
            return Ok(result);
        }

        // GET: api/rooms/admin?storeId=1
        // API cho Admin xem cả phòng ẩn
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] int? storeId)
        {
            var result = await _roomService.GetAllAsync(storeId, activeOnly: false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roomService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] RoomCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _roomService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _roomService.UpdateAsync(id, dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}