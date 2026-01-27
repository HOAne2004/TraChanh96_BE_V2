using drinking_be.Dtos.OrderItemDtos;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderDtos
{
    public class BaseOrderCreateDto
    {
        [Required]
        public int StoreId { get; set; }
        public int? PaymentMethodId { get; set; } // Nếu thanh toán online
        public string? UserNotes { get; set; }
        public string? VoucherCode { get; set; }

        [Required]
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
    // 1. DTO Tạo đơn Giao hàng
    public class DeliveryOrderCreateDto : BaseOrderCreateDto
    {
        [Required]
        public long DeliveryAddressId { get; set; } // ID địa chỉ trong sổ địa chỉ của User
    }

    // 2. DTO Tạo đơn Tại quầy
    public class AtCounterOrderCreateDto : BaseOrderCreateDto
    {
        public int? TableId { get; set; } // Có thể null nếu mang về
    }

    // 3. DTO tạo đơn Đến lấy
    public class PickupOrderCreateDto : BaseOrderCreateDto // Kế thừa các field chung như StoreId, Items, Note...
    {
        [Required]
        public int PaymentMethodId { get; set; }

        public string? VoucherCode { get; set; }

        [Required]
        public DateTime PickupTime { get; set; } // 🟢 Thời gian khách hẹn đến lấy
    }
}
