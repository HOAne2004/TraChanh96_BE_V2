using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.OrderDtos
{
    public class DeliveryOrderCreateDto : BaseOrderCreateDto
    {
        [Required(ErrorMessage = "Đơn giao hàng bắt buộc phải có địa chỉ")]
        public long DeliveryAddressId { get; set; }
    }
}
