using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.OrderDtos
{
    public class OrderCancelDto
    {
        [Required]
        public OrderCancelReasonEnum Reason { get; set; }

        // Nếu chọn "Lý do khác" (Other) thì bắt buộc phải nhập Note
        public string? Note { get; set; }
    }
}