using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.UserDtos
{
    // DTO trả về khi Login/Refresh thành công
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    // DTO gửi lên để làm mới token
    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    // DTO quên mật khẩu
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    // DTO đặt lại mật khẩu
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty; // Token nhận được từ email

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
    public class VerifyEmailDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
    public class ResendVerificationDto
    {
        public string Email { get; set; } = string.Empty;
    }
}