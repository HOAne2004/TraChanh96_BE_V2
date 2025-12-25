namespace drinking_be.Dtos.OrderDtos
{
    // DTO để lọc đơn hàng
    public class OrderFilterDto : Dtos.Common.PagingRequest
    {
        public int? StoreId { get; set; }
        public Enums.OrderStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    // DTO cập nhật trạng thái
    public class UpdateOrderStatusDto
    {
        public Enums.OrderStatusEnum Status { get; set; }
    }

    // DTO gán shipper
    public class AssignShipperDto
    {
        public int? ShipperId { get; set; } // Null = Tự nhận
    }

    // DTO xác thực lấy đồ
    public class VerifyPickupDto
    {
        public string PickupCode { get; set; } = null!;
    }

    // DTO thống kê nhanh
    public class OrderQuickStatsDto
    {
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int PendingOrders { get; set; } // Đơn chờ xác nhận
        public int ShippingOrders { get; set; } // Đơn đang giao
    }
}