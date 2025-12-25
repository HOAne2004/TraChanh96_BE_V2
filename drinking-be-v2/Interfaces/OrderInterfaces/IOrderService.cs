using drinking_be.Dtos.Common;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface IOrderService
    {
        // 1. Tạo đơn
        Task<OrderReadDto> CreateAtCounterOrderAsync(int? userId, AtCounterOrderCreateDto dto);
        Task<OrderReadDto> CreateDeliveryOrderAsync(int? userId, DeliveryOrderCreateDto dto);

        // 2. Xử lý đơn
        Task<OrderReadDto> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus);
        Task<bool> CancelOrderAsync(long orderId, int? userId, OrderCancelDto cancelDto);

        // 3. Tra cứu
        Task<PagedResult<OrderReadDto>> GetMyOrdersAsync(int userId, PagingRequest request);
        Task<OrderReadDto> GetOrderByIdAsync(long id);

        // 4. Vận hành (Shipper/Staff)
        Task<bool> AssignShipperAsync(long orderId, int shipperId);
        Task<bool> VerifyPickupCodeAsync(long orderId, string code);

        //5. Quản lý & Thống kê
        Task<PagedResult<OrderReadDto>> GetOrdersByFilterAsync(OrderFilterDto filter);
        Task<OrderQuickStatsDto> GetQuickStatsAsync(int? storeId, DateTime date);


    }
}