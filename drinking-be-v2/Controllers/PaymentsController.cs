using drinking_be.Data;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using drinking_be.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly DBDrinkContext _context;

    public PaymentsController(IVnPayService vnPayService, DBDrinkContext context)
    {
        _vnPayService = vnPayService;
        _context = context;
    }

    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn()
    {
        var queryCollection = Request.Query;

        // 1. Kiểm tra chữ ký bảo mật
        if (!_vnPayService.ValidateSignature(queryCollection))
        {
            return BadRequest("Chữ ký VNPay không hợp lệ!");
        }

        string orderCode = queryCollection["vnp_TxnRef"].ToString();
        string vnpResponseCode = queryCollection["vnp_ResponseCode"].ToString(); // "00" là thành công
        string transactionNo = queryCollection["vnp_TransactionNo"].ToString();

        // 2. Tìm đơn hàng
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        if (order == null) return NotFound("Không tìm thấy đơn hàng");

        // FrontEnd URL (Sửa port cho khớp với VueJS của bạn)
        string feRedirectUrl = $"http://localhost:5173/order/{orderCode}";

        // 3. Nếu giao dịch thành công ("00")
        if (vnpResponseCode == "00")
        {
            // Tránh update 2 lần nếu user F5 trình duyệt
            if (!order.IsPaid)
            {
                order.IsPaid = true;
                order.PaymentDate = DateTime.UtcNow;

                // Lưu lịch sử thanh toán vào bảng OrderPayment
                var paymentRecord = new OrderPayment
                {
                    OrderId = order.Id,
                    PaymentMethodId = order.PaymentMethodId ?? 0,
                    Amount = order.GrandTotal,
                    TransactionCode = transactionNo,
                    Status = OrderPaymentStatusEnum.Paid,
                    Type = OrderPaymentTypeEnum.Charge,
                    PaymentDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderPayments.Add(paymentRecord);
                await _context.SaveChangesAsync();
            }

            // Redirect về FE kèm query success để FE biết đường hiện Toast
            return Redirect($"{feRedirectUrl}?payment=success");
        }
        else
        {
            // Nếu giao dịch thất bại (Khách hủy, thiếu tiền...)
            return Redirect($"{feRedirectUrl}?payment=failed");
        }
    }
}