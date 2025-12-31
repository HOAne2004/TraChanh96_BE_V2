using drinking_be.Dtos.ReservationDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // --- Helper lấy UserID ---
        private int GetUserId()
        {
            return User.GetUserId();
        }

        // POST: api/reservation
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Tự động gán UserId nếu đang đăng nhập
            var userId = GetUserId();
            if (userId > 0 && dto.UserId == null)
            {
                dto.UserId = userId;
            }

            try
            {
                var result = await _reservationService.CreateReservationAsync(dto);
                return CreatedAtAction(nameof(GetReservationById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/reservation/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(long id)
        {
            var result = await _reservationService.GetReservationByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy thông tin đặt bàn." });
            return Ok(result);
        }

        // GET: api/reservation/my-history
        [HttpGet("my-history")]
        [Authorize]
        public async Task<IActionResult> GetMyHistory()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _reservationService.GetHistoryByUserIdAsync(userId);
            return Ok(result);
        }

        // GET: api/reservation/store/{storeId}
        [HttpGet("store/{storeId}")]
        [Authorize(Roles = "Admin,Manager,Staff")] // Chỉ nhân viên mới được xem danh sách quán
        public async Task<IActionResult> GetByStore(int storeId, [FromQuery] DateTime? date, [FromQuery] ReservationStatusEnum? status)
        {
            var result = await _reservationService.GetReservationsByStoreAsync(storeId, date, status);
            return Ok(result);
        }

        // PUT: api/reservation/{id} (Admin/Manager update)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateReservation(long id, [FromBody] ReservationUpdateDto updateDto)
        {
            try
            {
                var result = await _reservationService.UpdateReservationAsync(id, updateDto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/reservation/{id}/cancel (User hủy)
        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(long id, [FromBody] string reason)
        {
            var userId = GetUserId();
            try
            {
                var result = await _reservationService.CancelReservationAsync(id, userId, reason);
                if (!result) return BadRequest(new { message = "Không thể hủy đơn này." });
                return Ok(new { message = "Đã hủy đặt bàn thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}