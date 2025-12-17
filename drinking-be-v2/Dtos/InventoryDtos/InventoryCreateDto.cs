using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.InventoryDtos
{
    public class InventoryCreateDto
    {
        [Required(ErrorMessage = "Mã nguyên liệu không được để trống.")]
        public int MaterialId { get; set; }

        // Nếu null => Nhập vào Kho tổng
        // Nếu có giá trị => Nhập vào Kho cửa hàng cụ thể
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "Số lượng ban đầu không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm.")]
        public int Quantity { get; set; } = 0;
    }
}