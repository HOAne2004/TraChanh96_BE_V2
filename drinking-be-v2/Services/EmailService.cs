using drinking_be.Dtos.EmailDtos;
using FluentEmail.Core;

namespace drinking_be.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string username, string verificationLink, string token);
        Task SendResetPasswordEmailAsync(string toEmail, string username, string resetLink);
        Task SendOrderReceiptEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order);
        Task SendAdminPaymentAlertEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order, decimal paidAmount);
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

                var model = new
                {
                    Username = username,
                    CompanyName = "Trà Chanh 96",
                    Token = token,
                    VerificationLink = verificationLink
                };

                var response = await _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Xác thực tài khoản - Trà chanh 96")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true)
                    .Tag("Verification")
                    .SendAsync(); // GỌI TRỰC TIẾP TẠI ĐÂY

                if (!response.Successful)
                {
                    _logger.LogError($"[MAIL ERROR] Gửi email xác thực thất bại đến {toEmail}. Lỗi: {string.Join(", ", response.ErrorMessages)}");
                }
                else
                {
                    _logger.LogInformation($"[SUCCESS] Đã gửi mail xác thực đến: {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi gửi email xác thực đến {toEmail}");
            }
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string username, string resetLink)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "ResetPassword.cshtml");

                var model = new
                {
                    Username = username,
                    CompanyName = "Trà Chanh 96",
                    ResetLink = resetLink
                };

                var response = await _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Yêu cầu đặt lại mật khẩu - Trà chanh 96")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true)
                    .Tag("ResetPassword")
                    .SendAsync();

                if (!response.Successful)
                {
                    _logger.LogError($"[MAIL ERROR] Gửi email reset pass thất bại đến {toEmail}. Lỗi: {string.Join(", ", response.ErrorMessages)}");
                }
                else
                {
                    _logger.LogInformation($"[SUCCESS] Đã gửi mail reset pass đến: {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi gửi email reset pass đến {toEmail}");
            }
        }

        public async Task SendOrderReceiptEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "OrderReceipt.cshtml");
                string customerName = !string.IsNullOrEmpty(order.RecipientName) ? order.RecipientName : order.UserName;

                var model = new
                {
                    OrderCode = order.OrderCode,
                    OrderDate = (order.OrderDate ?? order.CreatedAt).AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                    CustomerName = customerName,
                    OrderType = order.OrderTypeLabel,
                    PaymentStatus = order.IsPaid ? "Đã thanh toán" : "Chưa thanh toán",
                    PaymentMethod = order.PaymentMethodName ?? "N/A",
                    TotalAmount = $"{order.TotalAmount:N0}đ",
                    ShippingFee = $"{order.ShippingFee ?? 0:N0}đ",
                    Discount = $"{order.DiscountAmount ?? 0:N0}đ",
                    GrandTotal = $"{order.GrandTotal:N0}đ",
                    Items = order.Items
                };

                var response = await _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"Hóa đơn điện tử - Đơn hàng #{order.OrderCode} - Trà Chanh 1996")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true)
                    .Tag("OrderReceipt")
                    .SendAsync();

                if (!response.Successful)
                {
                    _logger.LogError($"[MAIL ERROR] Gửi hóa đơn thất bại đến {toEmail}. Lỗi: {string.Join(", ", response.ErrorMessages)}");
                }
                else
                {
                    _logger.LogInformation($"[SUCCESS] Đã gửi hóa đơn đến: {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi gửi hóa đơn đến {toEmail}");
            }
        }

        public async Task SendAdminPaymentAlertEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order, decimal paidAmount)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "AdminPaymentAlert.cshtml");
                string customerName = !string.IsNullOrEmpty(order.RecipientName) ? order.RecipientName : order.UserName;
                string adminLink = $"https://tra-chanh-96.vercel.app/admin/orders/{order.OrderCode}";

                var model = new
                {
                    OrderCode = order.OrderCode,
                    CustomerName = customerName,
                    PaidAmount = $"{paidAmount:N0}đ",
                    PaymentTime = DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                    AdminOrderLink = adminLink
                };

                var response = await _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"[TING TING] Đơn #{order.OrderCode} đã thanh toán {paidAmount:N0}đ")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true)
                    .Tag("AdminAlert")
                    .SendAsync();

                if (!response.Successful)
                {
                    _logger.LogError($"[MAIL ERROR] Gửi báo cáo thanh toán thất bại đến {toEmail}. Lỗi: {string.Join(", ", response.ErrorMessages)}");
                }
                else
                {
                    _logger.LogInformation($"[SUCCESS] Đã gửi báo cáo thanh toán đến Admin: {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email báo cáo thanh toán cho admin {toEmail}");
            }
        }
    }
}