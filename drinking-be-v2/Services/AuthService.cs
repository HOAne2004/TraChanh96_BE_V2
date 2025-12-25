using AutoMapper;
using drinking_be.Dtos.UserDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace drinking_be.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        // private readonly IEmailService _emailService; // Cần inject EmailService để gửi mail thật

        public AuthService(IUnitOfWork unitOfWork, IJwtGenerator jwtGenerator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        public async Task<UserReadDto> RegisterAsync(UserRegisterDto registerDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var membershipRepo = _unitOfWork.Repository<Membership>();
            var levelRepo = _unitOfWork.Repository<MembershipLevel>();

            if (await userRepo.ExistsAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username))
            {
                throw new Exception("Email hoặc Tên đăng nhập đã được sử dụng.");
            }

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = PasswordHasher.HashPassword(registerDto.Password);
            user.RoleId = UserRoleEnum.Customer;
            user.Status = UserStatusEnum.Active;

            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(); // Commit User để lấy ID

            // Tạo Membership mặc định
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

            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<TokenResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            // Include Staff để JWT Generator lấy được StoreId
            var user = await userRepo.GetFirstOrDefaultAsync(
                filter: u => u.Email == loginDto.Email || u.Username == loginDto.Email,
                includeProperties: "Staff"
            );

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");
            }

            if (user.Status == UserStatusEnum.Locked)
            {
                throw new Exception("Tài khoản của bạn đã bị khóa.");
            }

            // Tạo Access Token & Refresh Token
            var accessToken = _jwtGenerator.CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            // Lưu Refresh Token vào DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30); // 30 ngày
            user.LastLogin = DateTime.UtcNow;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            // Tìm user có RefreshToken trùng khớp
            var user = await userRepo.GetFirstOrDefaultAsync(
                filter: u => u.RefreshToken == requestDto.RefreshToken,
                includeProperties: "Staff"
            );

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
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

            if (user == null)
            {
                // Vì lý do bảo mật, không nên báo lỗi nếu email không tồn tại
                return;
            }

            // Tạo Reset Token
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiryTime = DateTime.UtcNow.AddHours(1); // Hết hạn sau 1 giờ

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Gửi Email chứa link reset password
            // Ví dụ: _emailService.SendEmail(user.Email, "Reset Password", $"Mã của bạn là: {token}");
            // Trong môi trường Dev, bạn có thể Log token ra console hoặc trả về API (không khuyến khích)
            Console.WriteLine($"[DEBUG] Reset Token for {user.Email}: {token}");
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