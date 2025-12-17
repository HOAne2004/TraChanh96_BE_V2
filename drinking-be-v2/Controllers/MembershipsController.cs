using drinking_be.Dtos.MembershipDtos;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        // --- Helper: GetUserId (Phiên bản "bất tử" - sửa lỗi Token) ---
        private int GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c =>
                (c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier)
                && int.TryParse(c.Value, out _));

            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }

            // Fallback
            var subClaim = User.FindFirst("sub");
            if (subClaim != null && int.TryParse(subClaim.Value, out int subId))
            {
                return subId;
            }

            throw new UnauthorizedAccessException("Token không hợp lệ.");
        }

        /// <summary>
        /// [USER] Lấy thông tin thành viên (cấp độ, điểm, v.v.) của tôi.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMembership()
        {
            try
            {
                var userId = GetUserId();
                var membershipInfo = await _membershipService.GetMyMembershipAsync(userId);

                if (membershipInfo == null)
                {
                    return NotFound("Bạn chưa kích hoạt thành viên.");
                }

                return Ok(membershipInfo);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // Có thể thêm API Admin tạo thẻ cho User nếu cần (POST)
    }
}