using drinking_be.Dtos.OrderDtos;
using drinking_be.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo đơn hàng mới từ khách hàng hoặc khách vãng lai.
        /// </summary>
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderReadDto))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var createdOrder = await _orderService.CreateOrderAsync(orderDto);

        //        // Trả về 201 Created kèm theo thông tin đơn hàng đã tạo
        //        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi nghiệp vụ hoặc lỗi hệ thống
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi hệ thống: {ex.Message}");
        //    }
        //}

        //// ⭐️ THÊM MỚI: API Lấy danh sách đơn hàng (Hỗ trợ lọc theo User và Sắp xếp)
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetOrders(
        //    [FromQuery] int? userId, // Lọc theo UserId
        //    [FromQuery(Name = "_sort")] string? sortBy,   // Map "_sort" -> sortBy
        //    [FromQuery(Name = "_order")] string? sortOrder // Map "_order" -> sortOrder
        //)
        //{
        //    try
        //    {
        //        // Gọi Service để lấy dữ liệu (Bạn cần cập nhật IOrderService tương ứng)
        //        // Ví dụ logic xử lý sắp xếp đơn giản:
        //        var orders = await _orderService.GetOrdersAsync(userId);

        //        // Xử lý sắp xếp tại Controller hoặc đẩy xuống Service (khuyên dùng Service)
        //        if (!string.IsNullOrEmpty(sortBy))
        //        {
        //            if (sortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
        //            {
        //                orders = (sortOrder?.ToLower() == "desc")
        //                    ? orders.OrderByDescending(o => o.CreatedAt)
        //                    : orders.OrderBy(o => o.CreatedAt);
        //            }
        //            // Thêm các case sắp xếp khác nếu cần
        //        }

        //        return Ok(orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi lấy danh sách đơn hàng: {ex.Message}");
        //    }
        //}

        ///// <summary>
        ///// Lấy thông tin đơn hàng theo ID (placeholder)
        ///// </summary>
        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public IActionResult GetOrderById(long id)
        //{
        //    // Implement logic to retrieve order details here
        //    return Ok($"Đã tạo đơn hàng thành công với ID: {id}.");
        //}
    }
}