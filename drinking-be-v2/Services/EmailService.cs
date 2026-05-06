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

        // 1. GỬI EMAIL XÁC THỰC
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

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Xác thực tài khoản - Trà chanh 96")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true);

                SendEmailInBackground(email, "Xác thực tài khoản", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi chuẩn bị email xác thực đến {toEmail}");
            }
        }

        // 2. GỬI EMAIL ĐẶT LẠI MẬT KHẨU
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

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject("Yêu cầu đặt lại mật khẩu - Trà chanh 96")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true);

                SendEmailInBackground(email, "Đặt lại mật khẩu", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi chuẩn bị email reset pass đến {toEmail}");
            }
        }

        // 3. GỬI EMAIL HÓA ĐƠN
        public async Task SendOrderReceiptEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "OrderReceipt.cshtml");
                string customerName = !string.IsNullOrEmpty(order.RecipientName) ? order.RecipientName : order.UserName;

                // Bạn truyền thẳng toàn bộ object order vào Model. 
                // Xử lý vòng lặp các món hàng sẽ được thực hiện trực tiếp bên trong file .cshtml bằng cú pháp @foreach
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
                    Items = order.Items // Truyền danh sách món để xử lý trong Razor
                };

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"Hóa đơn điện tử - Đơn hàng #{order.OrderCode} - Trà Chanh 1996")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true);

                SendEmailInBackground(email, "Hóa đơn điện tử", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi hệ thống khi chuẩn bị hóa đơn đến {toEmail}");
            }
        }

        // 4. GỬI EMAIL THÔNG BÁO CHO ADMIN
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

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"[TING TING] Đơn #{order.OrderCode} đã thanh toán {paidAmount:N0}đ")
                    .UsingTemplateFromFile(templatePath, model, isHtml: true);

                SendEmailInBackground(email, "Báo cáo thanh toán Admin", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi chuẩn bị email báo cáo thanh toán cho admin {toEmail}");
            }
        }

        // --- HÀM HELPER: XỬ LÝ CHẠY NGẦM ĐỂ KHÔNG TREO API ---
        private void SendEmailInBackground(IFluentEmail email, string emailType, string toEmail)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var response = await email.SendAsync();
                    if (!response.Successful)
                    {
                        var errors = string.Join(", ", response.ErrorMessages);
                        _logger.LogError($"[Background] Gửi email '{emailType}' thất bại đến {toEmail}. Lỗi: {errors}");
                    }
                    else
                    {
                        _logger.LogInformation($"[SUCCESS] Đã gửi mail '{emailType}' thành công đến: {toEmail}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Background] Lỗi hệ thống thực thi ngầm khi gửi email '{emailType}' đến {toEmail}");
                }
            });
        }
    }
}