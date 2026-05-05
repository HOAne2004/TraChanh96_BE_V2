using drinking_be.Dtos.EmailDtos;
using FluentEmail.Core;

namespace drinking_be.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string username, string verificationLink, string token);
        Task SendResetPasswordEmailAsync(string toEmail, string username, string resetLink);
        Task SendOrderReceiptEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order);
        Task SendAdminPaymentAlertEmailAsync(string toEmail,Dtos.OrderDtos.OrderReadDto order, decimal paidAmount);
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
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "VerifyEmail.html");
                
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
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "ResetPassword.html");

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

        public async Task SendOrderReceiptEmailAsync(string toEmail, Dtos.OrderDtos.OrderReadDto order)
        {
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "OrderReceipt.html");
                string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);

                // Build danh sách món hàng (bao gồm Topping)
                var itemsHtml = new System.Text.StringBuilder();

                // Vòng lặp 1: Quét qua tất cả các món chính (Vì DTO đã tách riêng Topping ra rồi)
                foreach (var item in order.Items)
                {
                    // Món chính (Dùng thuộc tính TotalPrice thay vì FinalPrice * Quantity)
                    itemsHtml.Append($@"
            <tr>
                <td style='padding: 5px 0;'><strong>{item.ProductName}</strong><br/><span style='font-size:12px;color:#6b7280;'>({item.SizeName ?? "Vừa"})</span></td>
                <td style='text-align: center; padding: 5px 0;'>{item.Quantity}</td>
                <td style='text-align: right; padding: 5px 0;'>{item.TotalPrice:N0}đ</td>
            </tr>");

                    // Các Topping đi kèm (Quét qua danh sách Toppings trong DTO)
                    if (item.Toppings != null && item.Toppings.Any())
                    {
                        foreach (var topping in item.Toppings)
                        {
                            // LƯU Ý: Tôi đang giả định OrderToppingReadDto của bạn có các thuộc tính là ProductName, Quantity và TotalPrice. 
                            // Nếu tên biến trong DTO đó khác (ví dụ: Name, Price), bạn hãy sửa lại một chút ở dòng dưới nhé!
                            itemsHtml.Append($@"
                    <tr>
                        <td style='padding: 2px 0 2px 10px; font-size: 12px; color:#4b5563;'>+ {topping.ProductName}</td>
                        <td style='text-align: center; font-size: 12px; color:#4b5563;'>{topping.Quantity}</td>
                        <td style='text-align: right; font-size: 12px; color:#4b5563;'>{topping.FinalPrice:N0}đ</td>
                    </tr>");
                        }
                    }
                }

                // Tên khách hàng (Ưu tiên tên người nhận, nếu không có thì lấy tên User)
                string customerName = !string.IsNullOrEmpty(order.RecipientName) ? order.RecipientName : order.UserName;

                // Thay thế các biến trong Template
                templateContent = templateContent
                    .Replace("{{OrderCode}}", order.OrderCode)
                    .Replace("{{OrderDate}}", (order.OrderDate ?? order.CreatedAt).AddHours(7).ToString("dd/MM/yyyy HH:mm"))
                    .Replace("{{CustomerName}}", customerName)
                    .Replace("{{OrderType}}", order.OrderTypeLabel)
                    .Replace("{{PaymentStatus}}", order.IsPaid ? "Đã thanh toán" : "Chưa thanh toán")
                    .Replace("{{PaymentMethod}}", order.PaymentMethodName ?? "N/A")
                    .Replace("{{OrderItemsHtml}}", itemsHtml.ToString())
                    .Replace("{{TotalAmount}}", $"{order.TotalAmount:N0}đ")
                    .Replace("{{ShippingFee}}", $"{order.ShippingFee ?? 0:N0}đ")
                    .Replace("{{Discount}}", $"{order.DiscountAmount ?? 0:N0}đ")
                    .Replace("{{GrandTotal}}", $"{order.GrandTotal:N0}đ");

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"Hóa đơn điện tử - Đơn hàng #{order.OrderCode} - Trà Chanh 1996")
                    .Body(templateContent, isHtml: true);

                var response = await email.SendAsync();

                if (!response.Successful)
                {
                    _logger.LogError($"Gửi hóa đơn thất bại đến {toEmail}. Lỗi: {string.Join(", ", response.ErrorMessages)}");
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
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "AdminPaymentAlert.html");
                string templateContent = await File.ReadAllTextAsync(templatePath);

                string customerName = !string.IsNullOrEmpty(order.RecipientName) ? order.RecipientName : order.UserName;
                string adminLink = $"http://localhost:5173/admin/orders/{order.OrderCode}"; // Đổi port cho khớp FE Admin của bạn

                templateContent = templateContent
                    .Replace("{{OrderCode}}", order.OrderCode)
                    .Replace("{{CustomerName}}", customerName)
                    .Replace("{{PaidAmount}}", $"{paidAmount:N0}đ")
                    .Replace("{{PaymentTime}}", DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy HH:mm"))
                    .Replace("{{AdminOrderLink}}", adminLink);

                var email = _fluentEmailFactory
                    .Create()
                    .To(toEmail)
                    .Subject($"[TING TING] Đơn #{order.OrderCode} đã thanh toán {paidAmount:N0}đ")
                    .Body(templateContent, isHtml: true);

                await email.SendAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email báo cáo thanh toán cho admin {toEmail}");
            }
        }
    }
}