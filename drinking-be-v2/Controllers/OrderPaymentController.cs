using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [ApiController]
    [Route("api/order-payments")]
    public class OrderPaymentController : ControllerBase
    {
        private readonly IOrderPaymentService _orderPaymentService;

        public OrderPaymentController(IOrderPaymentService orderPaymentService)
        {
            _orderPaymentService = orderPaymentService;
        }

        // =========================================================
        // 1. Tạo giao dịch thanh toán
        // =========================================================
        [HttpPost("charge")]
        [Authorize] // nếu cho khách vãng lai thì bỏ
        public async Task<IActionResult> CreateCharge([FromBody] CreateChargeRequestDto dto)
        {
            var result = await _orderPaymentService.CreateChargeAsync(
                dto.OrderId,
                dto.PaymentMethodId
            );

            return Ok(result);
        }

        // =========================================================
        // 2. Callback: Thanh toán thành công
        // =========================================================
        [HttpPost("{paymentId}/success")]
        [AllowAnonymous] // gateway gọi
        public async Task<IActionResult> PaymentSuccess(
            long paymentId,
            [FromBody] PaymentCallbackDto dto)
        {
            var success = await _orderPaymentService.MarkPaymentSuccessAsync(
                paymentId,
                dto.TransactionCode
            );

            return Ok(new { success });
        }

        // =========================================================
        // 3. Callback: Thanh toán thất bại
        // =========================================================
        [HttpPost("{paymentId}/fail")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentFailed(
            long paymentId,
            [FromBody] PaymentCallbackDto dto)
        {
            var success = await _orderPaymentService.MarkPaymentFailedAsync(
                paymentId,
                dto.Reason ?? "Payment failed"
            );

            return Ok(new { success });
        }

        // =========================================================
        // 4. Hoàn tiền (Admin / System)
        // =========================================================
        [HttpPost("refund")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Refund([FromBody] RefundRequestDto dto)
        {
            var result = await _orderPaymentService.RefundAsync(
                dto.OrderId,
                dto.Amount,
                dto.Reason
            );

            return Ok(result);
        }
    }
}
