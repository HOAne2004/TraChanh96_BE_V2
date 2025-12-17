using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.InventoryDtos
{
    public class InventoryUpdateDto
    {
        // Chỉ cho phép sửa số lượng (Ví dụ: Kiểm kho thấy lệch)
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        public int Quantity { get; set; }
    }
}