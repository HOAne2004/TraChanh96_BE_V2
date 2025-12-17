using drinking_be.Dtos.OrderDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface IOrderService
    {
        // Tạo đơn hàng mới
        Task<OrderReadDto> CreateOrderAsync(int userId, OrderCreateDto dto);

        // Lấy chi tiết đơn hàng
        Task<OrderReadDto?> GetOrderByIdAsync(long orderId);

        // Lấy danh sách đơn hàng của User (Lịch sử mua hàng)
        Task<IEnumerable<OrderReadDto>> GetMyOrdersAsync(int userId, OrderStatusEnum? status);

        // Admin: Lấy tất cả đơn hàng (Quản lý)
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync(OrderStatusEnum? status, string? searchCode);

        // Cập nhật trạng thái đơn hàng (Duyệt, Giao, Hoàn thành...)
        Task<OrderReadDto?> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus);

        // Hủy đơn hàng
        Task<bool> CancelOrderAsync(long orderId, int userId, string reason);
    }
}