using drinking_be.Dtos.UserDtos;
using drinking_be.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            var user = await _authService.RegisterAsync(registerDto);
            return StatusCode(201, user);
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var result = await _authService.VerifyEmailAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("resend-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
        {
            await _authService.ResendVerificationCodeAsync(dto.Email);
            return Ok(new { message = "Nếu email tồn tại và chưa xác thực, mã mới đã được gửi." });
        }

        [EnableRateLimiting("loginPolicy")]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var tokenResponse = await _authService.LoginAsync(loginDto);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var tokenResponse = await _authService.RefreshTokenAsync(requestDto);
            return Ok(tokenResponse);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok(new { message = "Nếu email tồn tại, hệ thống đã gửi hướng dẫn đặt lại mật khẩu." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
        }
    }
}