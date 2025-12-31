using drinking_be.Dtos.VoucherDtos;
using drinking_be.Interfaces.MarketingInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserVouchersController : ControllerBase
    {
        private readonly IUserVoucherService _voucherService;

        public UserVouchersController(IUserVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        private int GetUserId()
        {
            return User.GetUserId();
        }

        [HttpGet("me")] // Route: /api/uservouchers/me
        public async Task<IActionResult> GetMyVouchers()
        {
            var vouchers = await _voucherService.GetMyVouchersAsync(GetUserId());
            return Ok(vouchers);
        }

        [HttpPost("apply")] // Route: /api/uservouchers/apply
        public async Task<IActionResult> ApplyVoucher([FromBody] VoucherApplyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _voucherService.ApplyVoucherAsync(GetUserId(), dto);

            if (!result.IsValid) return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        [HttpPost("issue")] // Route: /api/uservouchers/issue (Admin only)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> IssueVoucher([FromBody] UserVoucherCreateDto dto)
        {
            try
            {
                var result = await _voucherService.IssueVoucherAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}