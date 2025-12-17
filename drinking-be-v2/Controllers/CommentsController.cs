using drinking_be.Dtos.CommentDtos;
using drinking_be.Interfaces.FeedbackInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // --- Helper: GetUserId (Phiên bản chuẩn) ---
        private int GetUserId()
        {
            // 1. Tìm theo nameid (chuẩn JWT)
            var claim = User.FindFirst("nameid") ?? User.FindFirst(ClaimTypes.NameIdentifier);

            // 2. Fallback sang sub
            if (claim == null) claim = User.FindFirst("sub");

            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Token không hợp lệ.");
        }

        // --- PUBLIC ENDPOINTS ---

        /// <summary>
        /// Lấy danh sách bình luận của một bài viết (Chỉ hiện Approved).
        /// </summary>
        [HttpGet("news/{newsId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsByNews(int newsId)
        {
            var comments = await _commentService.GetCommentsByNewsIdAsync(newsId);
            return Ok(comments);
        }

        // --- USER ENDPOINTS ---

        /// <summary>
        /// [USER] Gửi bình luận mới.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var result = await _commentService.CreateCommentAsync(userId, dto);

                // Trả về 201 Created kèm thông báo hoặc object
                return CreatedAtAction(nameof(GetCommentsByNews), new { newsId = dto.NewsId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>
        /// [USER] Xóa bình luận của chính mình.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _commentService.DeleteCommentAsync(id, userId);

                if (!success) return NotFound("Bình luận không tồn tại hoặc bạn không có quyền xóa.");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        }
    }
}