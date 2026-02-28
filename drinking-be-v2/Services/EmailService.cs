using drinking_be.Dtos.EmailDtos;
using FluentEmail.Core;

namespace drinking_be.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string username, string verificationLink, string token);
        Task SendResetPasswordEmailAsync(string toEmail, string username, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IFluentEmailFactory _fluentEmailFactory;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmailFactory fluentEmailFactory, ILogger<EmailService> logger)
        {
            _fluentEmailFactory = fluentEmailFactory;
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string username, string verificationLink, string token)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "VerifyEmail.cshtml");
                
                string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);
                templateContent = templateContent
                    .Replace("@model drinking_be.Dtos.EmailDtos.VerifyEmailModel", "")
                    .Replace("@Model.Username", username)
                    .Replace("@Model.CompanyName", "Trà Chanh 96")
                    .Replace("@Model.Token", token)
                    .Replace("@Model.VerificationLink", verificationLink);

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Xác thực tài khoản - Trà chanh 96")
                    .Body(templateContent, isHtml: true);

                var response = await email.SendAsync();

                if (!response.Successful)
                {
                    var errors = string.Join(", ", response.ErrorMessages);
                    _logger.LogError($"Gửi email thất bại đến {toEmail}. Lỗi: {errors}");
                    throw new Exception($"Không thể gửi email xác thực: {errors}");
                }

                _logger.LogInformation($"[SUCCESS] Đã gửi mail thành công đến: {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi gửi email đến {toEmail}");
                throw; 
            }
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string username, string resetLink)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "ResetPassword.cshtml");

                string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);
                templateContent = templateContent
                    .Replace("@model drinking_be.Dtos.EmailDtos.ResetPasswordEmailModel", "")
                    .Replace("@Model.Username", username)
                    .Replace("@Model.CompanyName", "Trà Chanh 96")
                    .Replace("@Model.ResetLink", resetLink);

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Yêu cầu đặt lại mật khẩu - Trà chanh 96")
                    .Body(templateContent, isHtml: true);

                var response = await email.SendAsync();

                if (!response.Successful)
                {
                    var errors = string.Join(", ", response.ErrorMessages);
                    _logger.LogError($"Gửi email reset pass thất bại đến {toEmail}. Lỗi: {errors}");
                    throw new Exception($"Không thể gửi email: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi gửi email reset pass đến {toEmail}");
                throw;
            }
        }
    }
}