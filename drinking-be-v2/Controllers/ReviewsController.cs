using drinking_be.Dtos.ReviewDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.FeedbackInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private int GetUserId()
        {
            return User.GetUserId();
        }

        // 1. Lấy đánh giá công khai của sản phẩm (Ai cũng xem được)
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetApprovedReviewsAsync(productId);
            return Ok(reviews);
        }

        // 2. Kiểm tra xem User có quyền review món này không (Để FE disable nút)
        [HttpGet("can-review/{productId}")]
        [Authorize]
        public async Task<IActionResult> CheckCanReview(int productId)
        {
            var canReview = await _reviewService.CanReviewAsync(GetUserId(), productId);
            return Ok(new { canReview });
        }

        // 3. Tạo Review (Cần OrderId trong DTO)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(GetUserId(), dto);
                return Ok(createdReview);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); } // Lỗi logic nghiệp vụ
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        // 4. User sửa Review của mình
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUserEditDto dto)
        {
            try
            {
                var updatedReview = await _reviewService.UpdateReviewByUserAsync(id, GetUserId(), dto);
                return Ok(updatedReview);
            }
            catch (KeyNotFoundException) { return NotFound(new { message = "Không tìm thấy đánh giá." }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // 5. Xóa Review (User xóa của mình, Admin xóa tất)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");
            try
            {
                var result = await _reviewService.DeleteReviewAsync(id, GetUserId(), isAdmin);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // --- ADMIN SECTION ---

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] int? productId, [FromQuery] ReviewStatusEnum? status)
        {
            var result = await _reviewService.GetAllReviewsAsync(productId, status);
            return Ok(result);
        }

        [HttpPut("{id}/admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AdminUpdate(int id, [FromBody] ReviewAdminUpdateDto dto)
        {
            try
            {
                var result = await _reviewService.UpdateReviewByAdminAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}