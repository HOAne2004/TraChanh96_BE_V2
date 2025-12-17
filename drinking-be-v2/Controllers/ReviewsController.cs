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

        // --- Helper GetUserId ---
        private int GetUserId()
        {
            var claim = User.FindFirst("nameid") ?? User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId)) return userId;
            return 0;
        }

        // --- PUBLIC ---

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetApprovedReviewsAsync(productId);
            return Ok(reviews);
        }

        // --- USER ---

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto reviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var createdReview = await _reviewService.CreateReviewAsync(userId, reviewDto);
                return CreatedAtAction(nameof(GetReviewsForProduct), new { productId = reviewDto.ProductId }, createdReview);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var result = await _reviewService.DeleteReviewAsync(id, userId, isAdmin);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // --- ADMIN ---

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] int? productId, [FromQuery] ReviewStatusEnum? status)
        {
            var result = await _reviewService.GetAllReviewsAsync(productId, status);
            return Ok(result);
        }

        [HttpPut("{id}/admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AdminUpdate(int id, [FromBody] ReviewUpdateDto dto)
        {
            var result = await _reviewService.UpdateReviewAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}