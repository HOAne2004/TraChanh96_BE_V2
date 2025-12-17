// File: Dtos/OrderDtos/OrderCreateDto.cs (Đã chuẩn hóa)

using System.ComponentModel.DataAnnotations;
using drinking_be.Dtos.OrderItemDtos;

namespace drinking_be.Dtos.OrderDtos
{
    public class OrderCreateDto
    {
        // Thông tin Đặt hàng
        public int? UserId { get; set; } // Có thể NULL

        [Required(ErrorMessage = "Mã cửa hàng không được để trống.")]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Mã phương thức thanh toán không được để trống.")]
        public int PaymentMethodId { get; set; }

        // ⭐ Khóa ngoại tới Address chuẩn hóa
        [Required(ErrorMessage = "Mã địa chỉ giao hàng không được để trống.")]
        public long DeliveryAddressId { get; set; }

        // ⭐ Voucher (chấp nhận Code hoặc ID)
        public long? UserVoucherId { get; set; }
        public string? VoucherCodeUsed { get; set; }

        // Ghi chú từ khách hàng
        [MaxLength(500)]
        public string? UserNotes { get; set; }

        // Danh sách các món hàng (bao gồm Topping lồng nhau)
        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một món.")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
    }
}