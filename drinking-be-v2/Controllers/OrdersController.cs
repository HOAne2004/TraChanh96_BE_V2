using drinking_be.Dtos.Common;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.OrderInterfaces;
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

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ==========================================================
        // 1. NHÓM TẠO ĐƠN
        // ==========================================================

        [HttpPost("delivery")]
        public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderCreateDto dto)
        {
            var userId = GetCurrentUserId(); // Lấy ID từ Token
            var result = await _orderService.CreateDeliveryOrderAsync(userId, dto);

            // TODO: Nếu là thanh toán Online, gọi VNPay Service lấy URL và gán vào result
            // if (result.PaymentMethod.Type == PaymentType.EWallet) { result.PaymentUrl = ... }

            return Ok(result);
        }

        [HttpPost("at-counter")]
        public async Task<IActionResult> CreateAtCounterOrder([FromBody] AtCounterOrderCreateDto dto)
        {
            // Tại quầy có thể không cần User đăng nhập (Khách vãng lai)
            // Nhưng nếu Staff tạo hộ khách thì UserId có thể null hoặc là ID của Staff
            int? userId = null;
            try
            {
                userId = GetCurrentUserId();
            }
            catch { } // Bỏ qua nếu không lấy được User

            var result = await _orderService.CreateAtCounterOrderAsync(userId, dto);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(long id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            // Bảo mật: Nếu là Customer, chỉ xem được đơn của chính mình
            var userId = GetCurrentUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Customer" && result.UserId != userId)
            {
                return Forbid("Bạn không có quyền xem đơn hàng này.");
            }

            return Ok(result);
        }

        // ==========================================================
        // 3. THỐNG KÊ NHANH (Theo yêu cầu của bạn)
        // ==========================================================
        [HttpGet("stats")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> GetQuickStats([FromQuery] int? storeId, [FromQuery] DateTime? date)
        {
            // Nếu là StoreManager, ép buộc storeId phải là store của họ (xử lý ở Service)
            var stats = await _orderService.GetQuickStatsAsync(storeId, date ?? DateTime.Today);
            return Ok(stats);
        }

        // ==========================================================
        // 4. XỬ LÝ TRẠNG THÁI (STAFF)
        // ==========================================================

        // Duyệt đơn / Nấu xong / Đang giao...
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
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

        // Xác thực mã lấy đồ (AtCounter)
        [HttpPut("{id}/verify-pickup")]
        [Authorize(Roles = "Admin,StoreManager,Staff")]
        public async Task<IActionResult> VerifyPickup(long id, [FromBody] VerifyPickupDto dto)
        {
            var success = await _orderService.VerifyPickupCodeAsync(id, dto.PickupCode);
            if (!success) return BadRequest("Mã lấy đồ không đúng.");

            return Ok(new { message = "Xác thực thành công. Giao đồ cho khách." });
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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.FindFirst("id"); // Hoặc ClaimTypes.NameIdentifier tùy cấu hình Token
                if (userClaim != null && int.TryParse(userClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            throw new UnauthorizedAccessException("Không tìm thấy thông tin User.");
        }
    }
}