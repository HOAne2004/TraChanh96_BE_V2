using drinking_be.Dtos.ChatDtos;
using drinking_be.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IAIService _aiService;

        public ChatController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
        {
            // 1. Trích xuất UserId từ Token (Nếu khách đã đăng nhập)
            // Lưu ý: Đổi ClaimTypes.NameIdentifier thành claim tương ứng mà JwtGenerator của bạn đang dùng (VD: "id", "userId"...)
            int? userId = null;

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            // 2. GỌI SERVICE XỬ LÝ
            var response = await _aiService.SendMessageAsync(
                request.StoreId,
                request.SessionId,
                userId,
                request.Message
            );

            // 3. TRẢ KẾT QUẢ CHO FRONTEND
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response); // Trả về lỗi 400 nếu hệ thống AI hoặc DB gặp sự cố
        }

        [HttpPost("admin/generate-content")]
        [Authorize(Roles = "Admin")] // Bảo mật endpoint
        public async Task<IActionResult> GenerateContent([FromBody] GenerateContentRequest request)
        {
            var result = await _aiService.GenerateMarkdownContentAsync(request.Prompt, request.ContentType);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}