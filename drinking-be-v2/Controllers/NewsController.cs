using drinking_be.Dtos.NewsDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // --- Helper: GetUserId ---
        private int GetUserId()
        {
            return User.GetUserId();
        }

        // --- PUBLIC ENDPOINTS ---

        [HttpGet("/api/carousel")]
        [AllowAnonymous]
        public IActionResult GetCarousel()
        {
            var slides = new List<object>
            {
                new { id = 1, imageUrl = "https://img.freepik.com/free-vector/flat-design-bubble-tea-banner-template_23-2149463197.jpg", title = "Chào hè rực rỡ", link = "/menu" },
                new { id = 2, imageUrl = "https://img.freepik.com/free-vector/flat-bubble-tea-banner-template_23-2149463198.jpg", title = "Món mới: Trà sữa nướng", link = "/products/tra-sua-nuong" }
            };
            return Ok(slides);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublishedNews()
        {
            var result = await _newsService.GetPublishedNewsAsync();
            return Ok(result);
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _newsService.GetNewsBySlugAsync(slug);
            if (result == null) return NotFound("Bài viết không tồn tại.");
            return Ok(result);
        }

        // --- ADMIN ENDPOINTS ---

        [HttpGet("admin")] // Route: /api/news/admin
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllNews([FromQuery] string? search, [FromQuery] ContentStatusEnum? status)
        {
            var result = await _newsService.GetAllNewsAsync(search, status);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] NewsCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId();
                var result = await _newsService.CreateNewsAsync(userId, dto);
                return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] NewsUpdateDto dto)
        {
            var result = await _newsService.UpdateNewsAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _newsService.DeleteNewsAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}