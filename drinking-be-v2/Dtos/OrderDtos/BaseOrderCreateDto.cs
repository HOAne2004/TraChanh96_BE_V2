using drinking_be.Dtos.OrderItemDtos;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderDtos
{
    public class BaseOrderCreateDto
    {
        [Required]
        public int StoreId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? UserNotes { get; set; }
        public string? VoucherCode { get; set; }

        [Required]
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
    // 1. DTO Tạo đơn Giao hàng
    public class DeliveryOrderCreateDto : BaseOrderCreateDto
    {
        [Required]
        public long DeliveryAddressId { get; set; } 
    }

    // 2. DTO Tạo đơn Tại quầy
    public class AtCounterOrderCreateDto : BaseOrderCreateDto
    {
        public int? TableId { get; set; } 
    }

    // 3. DTO tạo đơn Đến lấy
    public class PickupOrderCreateDto : BaseOrderCreateDto
    {
        [Required]
        public new int PaymentMethodId { get; set; }
        public new string? VoucherCode { get; set; }
        [Required]
        public DateTime PickupTime { get; set; } 
    }
}
