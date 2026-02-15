using drinking_be.Dtos.Common;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Mặc định tất cả phải đăng nhập
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderPaymentService _orderPaymentService;

        public OrdersController(IOrderService orderService, IOrderPaymentService orderPaymentService)
        {
            _orderService = orderService;
            _orderPaymentService = orderPaymentService;
        }

        // ==========================================================
        // 1. NHÓM TẠO ĐƠN
        // ==========================================================

        [HttpPost("delivery")]
        public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderCreateDto dto)
        {
            var userId = GetCurrentUserId(); // Lấy ID từ Token
            var result = await _orderService.CreateDeliveryOrderAsync(userId, dto);

            return Ok(result);
        }

        [HttpPost("at-counter")]
        public async Task<IActionResult> CreateAtCounterOrder([FromBody] AtCounterOrderCreateDto dto)
        {
            int? userId = User.Identity?.IsAuthenticated == true
                ? GetCurrentUserId()
                : null;

            var result = await _orderService.CreateAtCounterOrderAsync(userId, dto);
            return Ok(result);
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> CreatePickupOrder([FromBody] PickupOrderCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.CreatePickupOrderAsync(userId, dto);

            // Nếu là thanh toán Online
            // if (result.PaymentMethod.Type == PaymentType.EWallet) ...

            return Ok(result);
        }


        // ==========================================================
        // 2. NHÓM TRA CỨU & QUẢN LÝ
        // ==========================================================

        // Lấy danh sách đơn của CHÍNH TÔI (Customer)
        [HttpGet("me")]
        public async Task<IActionResult> GetMyOrders([FromQuery] PagingRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.GetMyOrdersAsync(userId, request);
            return Ok(result);
        }

        // Lấy danh sách đơn toàn hệ thống (Dành cho Staff/Manager)
        [HttpGet]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilterDto filter)
        {
            // StoreManager chỉ xem được đơn của Store mình (Logic này sẽ check trong Service)
            // Admin thấy hết.
            var result = await _orderService.GetOrdersByFilterAsync(filter);
            return Ok(result);
        }

        [HttpGet("{code}")] 
        public async Task<IActionResult> GetOrderByCode(string code)
        {
            // Nếu code gửi lên là số (cũ), vẫn support hoặc chặn tùy bạn. 
            // Ở đây ta ưu tiên tìm theo Code string.

            var result = await _orderService.GetOrderByOrderCodeAsync(code); // Sửa gọi hàm service

            // Validate quyền sở hữu
            if (User.IsInRole("Customer"))
            {
                var userId = GetCurrentUserId();
                if (result.UserId != userId) return Forbid();
            }

            return Ok(result);
        }


        // Thêm API mới
        [HttpGet("shipping-fee")]
        public async Task<IActionResult> GetShippingFee([FromQuery] int storeId, [FromQuery] long addressId)
        {
            try
            {
                var fee = await _orderService.CalculateShippingFeeAsync(storeId, addressId);
                return Ok(new { fee });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================================
        // 3. THỐNG KÊ NHANH
        // ==========================================================
        [HttpGet("stats")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> GetQuickStats([FromQuery] int? storeId, [FromQuery] DateTime? date)
        {
            // Truyền null nếu không chọn ngày, Service sẽ tự lấy ngày hiện tại của VN
            var stats = await _orderService.GetQuickStatsAsync(storeId, date);
            return Ok(stats);
        }

        // ==========================================================
        // 4. XỬ LÝ TRẠNG THÁI (STAFF)
        // ==========================================================

        // Duyệt đơn / Nấu xong / Đang giao...
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto)
        {
            var roleStr = User.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedAccessException();

            var role = Enum.Parse<UserRoleEnum>(roleStr);

            var result = await _orderService.UpdateOrderStatusAsync(
                id,
                dto.Status,
                role
            );

            return Ok(result);
        }


        // Gán Shipper (Nhân viên tự nhận hoặc Quản lý gán)
        [HttpPut("{id}/assign-shipper")]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> AssignShipper(long id, [FromBody] AssignShipperDto dto)
        {
            // Nếu dto.ShipperId null -> Lấy ID người đang gọi API (Tự nhận đơn)
            int shipperId = dto.ShipperId ?? GetCurrentUserId();

            var success = await _orderService.AssignShipperAsync(id, shipperId);
            if (!success) return BadRequest("Không thể gán shipper (Đơn không hợp lệ hoặc shipper bận).");

            return Ok(new { message = "Gán shipper thành công" });
        }

        // 🟢 CẬP NHẬT API VERIFY (Validate kỹ hơn)
        [HttpPut("{id}/verify-pickup")]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> VerifyPickup(long id, [FromBody] VerifyPickupDto dto)
        {
            try
            {
                var success = await _orderService.VerifyPickupCodeAsync(id, dto.PickupCode);
                // VerifyPickupCodeAsync sẽ throw exception nếu fail, nên nếu chạy qua được dòng trên là OK
                return Ok(new { message = "Xác thực thành công. Đã cập nhật trạng thái đơn hàng." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy đơn hàng hoặc mã xác thực sai.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ==========================================================
        // 5. HỦY ĐƠN (CUSTOMER)
        // ==========================================================
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(long id, [FromBody] OrderCancelDto dto)
        {
            var userId = GetCurrentUserId();
            var success = await _orderService.CancelOrderAsync(id, userId, dto);

            if (!success) return BadRequest("Không thể hủy đơn (Đơn đã được xác nhận hoặc không tồn tại).");

            return Ok(new { message = "Hủy đơn hàng thành công." });
        }

        // --- HELPER: Lấy UserId từ Token ---
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Token không chứa UserId.");

            return int.Parse(userIdClaim.Value);
        }

        // ==========================================================
        // 6. QUẢN LÝ THÙNG RÁC (ADMIN/MANAGER)
        // ==========================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,StoreManager")] // Chỉ quản lý mới được xóa
        public async Task<IActionResult> SoftDelete(long id)
        {
            var success = await _orderService.SoftDeleteOrderAsync(id);
            if (!success) return NotFound("Không tìm thấy đơn hàng.");
            return Ok(new { message = "Đã chuyển đơn hàng vào thùng rác." });
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Restore(long id)
        {
            var success = await _orderService.RestoreOrderAsync(id);
            if (!success) return NotFound("Không tìm thấy đơn hàng trong thùng rác.");
            return Ok(new { message = "Đã khôi phục đơn hàng thành công." });
        }

        // 7. XÁC NHẬN THANH TOÁN THỦ CÔNG (Dành cho Staff/Admin khi check thấy tiền về)
        [HttpPut("{id}/confirm-payment")]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> ConfirmPayment(long id)
        {
            // 1. Lấy đơn hàng kèm PaymentMethod
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            // 2. Tính số tiền còn thiếu
            var paymentSnapshot = await _orderPaymentService.BuildPaymentSnapshotAsync(id);
            decimal amountMissing = order.GrandTotal - paymentSnapshot.PaidAmount;

            if (amountMissing <= 0)
            {
                return BadRequest("Đơn hàng này đã được thanh toán đủ trước đó.");
            }

            // 3. Tạo giao dịch thanh toán (Paid)
            await _orderPaymentService.AutoConfirmPaymentAsync(
                order.Id,
                order.PaymentMethod?.Id ?? 0,
                order.PaymentMethod?.Name ?? "Unknown",
                amountMissing,
                $"Nhân viên {User.Identity.Name} xác nhận thủ công."
            );

            // 🟢 4. [FIX LOGIC] CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
            // Nếu đơn đang treo ở "Chờ thanh toán", chuyển nó sang "Mới" để quy trình tiếp tục
            if (order.Status == OrderStatusEnum.PendingPayment)
            {
                // Gọi service để update status (để trigger notification, log, etc nếu có)
                // Lưu ý: UserRoleEnum lấy từ Token (đã có code mẫu ở hàm UpdateStatus)
                var roleStr = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";
                var role = Enum.Parse<UserRoleEnum>(roleStr);

                await _orderService.UpdateOrderStatusAsync(order.Id, OrderStatusEnum.New, role);
            }

            return Ok(new { message = "Đã xác nhận thanh toán thành công." });
        }
    }
}