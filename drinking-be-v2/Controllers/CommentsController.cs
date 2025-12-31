using drinking_be.Dtos.CommentDtos;
using drinking_be.Interfaces.FeedbackInterfaces; // Ensure correct namespace
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

        // --- Helper: GetUserId (Robust Version) ---
        private int GetUserId()
        {
            return User.GetUserId();
        }

        // 1. Get Comments
        [HttpGet("news/{newsId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsByNews(int newsId)
        {
            // Try to get UserId if logged in (to check IsLiked status)
            // If not logged in, it returns 0 or throws exception -> catch and set to null
            int? currentUserId = null;
            try
            {
                var uid = GetUserId();
                if (uid > 0) currentUserId = uid;
            }
            catch { }

            var comments = await _commentService.GetCommentsByNewsIdAsync(newsId, currentUserId);
            return Ok(comments);
        }

        // 2. Create Comment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == 0) return Unauthorized("Token invalid");

            try
            {
                var result = await _commentService.CreateCommentAsync(userId, dto);
                return CreatedAtAction(nameof(GetCommentsByNews), new { newsId = dto.NewsId }, result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // 3. Delete Comment
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");

            try
            {
                var success = await _commentService.DeleteCommentAsync(id, userId, isAdmin);
                if (!success) return NotFound("Not found or unauthorized");
                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // 4. Toggle Like (New Endpoint)
        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            try
            {
                var isLiked = await _commentService.ToggleLikeAsync(id, userId);
                return Ok(new
                {
                    isLiked,
                    message = isLiked ? "Liked" : "Unliked",
                    // You might want to return the new Count here if needed, but usually FE handles optimistic UI
                });
            }
            catch (KeyNotFoundException) { return NotFound("Comment not found"); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}