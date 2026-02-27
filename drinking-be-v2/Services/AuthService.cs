using AutoMapper;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.UserDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace drinking_be.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IJwtGenerator jwtGenerator, IMapper mapper, IEmailService emailService, IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<UserReadDto> RegisterAsync(UserRegisterDto registerDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var membershipRepo = _unitOfWork.Repository<Membership>();
            var levelRepo = _unitOfWork.Repository<MembershipLevel>();

            if (await userRepo.ExistsAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("Email đăng nhập đã được sử dụng.");
            }

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = PasswordHasher.HashPassword(registerDto.Password);
            user.RoleId = UserRoleEnum.Customer;
            user.Status = UserStatusEnum.Active;
            user.EmailVerified = false;

            var token = new Random().Next(100000, 999999).ToString();
            user.VerificationToken = token;
            user.VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var levels = await levelRepo.GetAllAsync(orderBy: q => q.OrderBy(l => l.MinCoinsRequired));
            var baseLevel = levels.FirstOrDefault();

            if (baseLevel != null)
            {
                var newMembership = new Membership
                {
                    UserId = user.Id,
                    MembershipLevelId = baseLevel.Id,
                    CardCode = $"DRK{DateTime.Now.Year}{new Random().Next(100000, 999999)}",
                    TotalSpent = 0,
                    LevelStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    LevelEndDate = baseLevel.DurationDays.HasValue
                            ? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(baseLevel.DurationDays.Value)
                            : null
                };
                await membershipRepo.AddAsync(newMembership);
                await _unitOfWork.SaveChangesAsync();
            }

            try
            {
                var clientUrl = _configuration["ClientAppUrl"] ?? "http://localhost:5173";
                var verificationLink = $"{clientUrl}/verify-email?email={user.Email}&token={token}";

                await _emailService.SendVerificationEmailAsync(
                    user.Email,
                    user.Username ?? "Khách hàng mới",
                    verificationLink,
                    token
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi gửi mail khi đăng ký: {ex.Message}");
            }

            return _mapper.Map<UserReadDto>(user);
        }
        public async Task<ServiceResponse<string>> VerifyEmailAsync(VerifyEmailDto dto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            // 1. Tìm user theo ID
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "Tài khoản không tồn tại." };
            }

            // 2. Kiểm tra tài khoản đã xác thực chưa
            if (user.EmailVerified)
            {
                return new ServiceResponse<string> { Success = true, Message = "Tài khoản này đã được xác thực trước đó." };
            }

            // 3. Kiểm tra Token
            // - Token phải khớp
            // - Token chưa hết hạn (ExpiresAt > Now)
            if (user.VerificationToken != dto.Token)
            {
                return new ServiceResponse<string> { Success = false, Message = "Mã xác thực không hợp lệ." };
            }

            if (user.VerificationTokenExpiresAt < DateTime.UtcNow)
            {
                return new ServiceResponse<string> { Success = false, Message = "Mã xác thực đã hết hạn. Vui lòng yêu cầu gửi lại mã mới." };
            }

            // 4. Xác thực thành công
            user.EmailVerified = true;
            user.VerificationToken = null; // Xóa token để không dùng lại được
            user.VerificationTokenExpiresAt = null;
            user.Status = Enums.UserStatusEnum.Active; // Đảm bảo user active

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<string> { Success = true, Message = "Xác thực email thành công!" };
        }
        public async Task ResendVerificationCodeAsync(string email)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.EmailVerified)
            {
                return;
            }

            var token = new Random().Next(100000, 999999).ToString();
            user.VerificationToken = token;
            user.VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Gửi lại email
            try
            {
                var clientUrl = _configuration["ClientAppUrl"] ?? "http://localhost:5173";
                var verificationLink = $"{clientUrl}/verify-email?email={user.Email}&token={token}";

                await _emailService.SendVerificationEmailAsync(user.Email, user.Username, verificationLink, user.VerificationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Resend email failed: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<TokenResponseDto>> LoginAsync(UserLoginDto loginDto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(
                filter: u => u.Email == loginDto.Email || u.Username == loginDto.Email,
                includeProperties: "Staff"
            );

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return new ServiceResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không chính xác."
                };
            }

            if (!user.EmailVerified)
            {
                return new ServiceResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Tài khoản chưa được xác thực.",
                };
            }

            if (user.Status == UserStatusEnum.Locked)
            {
                return new ServiceResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Tài khoản đã bị khóa."
                };
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception("Tài khoản đang bị khóa tạm thời.");
            }
            else
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                }
            }
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            var accessToken = _jwtGenerator.CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            user.LastLogin = DateTime.UtcNow;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<TokenResponseDto>
            {
                Success = true,
                Data = new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }
        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(u => u.RefreshToken != null);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(requestDto.RefreshToken, user.RefreshToken) ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Refresh Token không hợp lệ hoặc đã hết hạn.");
            }

            // Tạo cặp token mới
            var newAccessToken = _jwtGenerator.CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Cập nhật lại DB (Thu hồi token cũ)
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokenResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }
        public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);

            if (user == null) return; 

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiryTime = DateTime.UtcNow.AddHours(1);

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                var clientUrl = _configuration["ClientAppUrl"] ?? "http://localhost:5173";

                var resetLink = $"{clientUrl}/reset-password?token={token}";

                // Gọi Email Service
                await _emailService.SendResetPasswordEmailAsync(
                    user.Email,
                    user.Username ?? "Khách hàng",
                    resetLink
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi gửi mail Reset Pass: {ex.Message}");
            }
        }
        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.ResetPasswordToken == resetPasswordDto.Token);

            if (user == null || user.ResetPasswordTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Token không hợp lệ hoặc đã hết hạn.");
            }

            // Đặt lại mật khẩu
            user.PasswordHash = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);

            // Xóa token sau khi dùng
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiryTime = null;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        // Hàm phụ trợ tạo Refresh Token ngẫu nhiên
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}