using drinking_be.Dtos.OrderItemDtos;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderDtos
{
    public class BaseOrderCreateDto
    {
        [Required]
        public int StoreId { get; set; }

        public string? UserNotes { get; set; }

        public int? PaymentMethodId { get; set; }

        // Logic Voucher: Ưu tiên dùng ID nếu chọn từ ví, dùng Code nếu nhập tay
        public long? UserVoucherId { get; set; }
        public string? VoucherCode { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 món")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
    }
}
