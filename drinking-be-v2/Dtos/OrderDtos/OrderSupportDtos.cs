using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderDtos
{
    // DTO để lọc đơn hàng
    public class OrderFilterDto : Dtos.Common.PagingRequest
    {
        public string? Keyword { get; set; }
        public int? StoreId { get; set; }
        public Enums.OrderStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
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
        [Required]
        [MaxLength(20)]
        public string PickupCode { get; set; } = null!;
    }

    // DTO thống kê nhanh
    public class OrderQuickStatsDto
    {
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int PendingOrders { get; set; } // Đơn chờ xác nhận
        public int ShippingOrders { get; set; } // Đơn đang giao
        public decimal TotalRevenueAllTime {get; set;}
        public int TotalCompletedOrders { get; set; }
    }
}