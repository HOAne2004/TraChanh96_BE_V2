using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.SupplyOrderDtos
{
    public class SupplyOrderUpdateDto
    {
        // Thay đổi trạng thái là hành động chính (Pending -> Approved -> Received)
        public SupplyOrderStatusEnum? Status { get; set; }

        public string? Note { get; set; } // Admin ghi chú: "Đã gửi xe Thành Bưởi"

        public DateTime? ExpectedDeliveryDate { get; set; }

        // Khi Manager nhận hàng, có thể cập nhật ngày thực nhận
        public DateTime? ReceivedAt { get; set; }
    }
}