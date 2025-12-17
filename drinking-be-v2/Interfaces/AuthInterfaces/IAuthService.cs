using drinking_be.Dtos.UserDtos;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        Task<UserReadDto> RegisterAsync(UserRegisterDto registerDto);

        // Login trả về cả Access và Refresh Token
        Task<TokenResponseDto> LoginAsync(UserLoginDto loginDto);

        // Làm mới Token
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto);

        // Quên mật khẩu (Trả về string message hoặc void)
        Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        // Đặt lại mật khẩu
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}