using drinking_be.Dtos.Common;
using drinking_be.Dtos.UserDtos;

namespace drinking_be.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        Task<UserReadDto> RegisterAsync(UserRegisterDto registerDto);
        Task<ServiceResponse<string>> VerifyEmailAsync(VerifyEmailDto dto);
        Task ResendVerificationCodeAsync(string email);
        Task<ServiceResponse<TokenResponseDto>> LoginAsync(UserLoginDto loginDto);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto);
        Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}