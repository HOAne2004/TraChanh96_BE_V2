using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.MaterialDtos
{
    public class MaterialCreateDto
    {
        [Required(ErrorMessage = "Tên nguyên liệu không được để trống.")]
        [MaxLength(200, ErrorMessage = "Tên nguyên liệu không quá 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        // --- Đơn vị & Quy đổi ---
        [Required(ErrorMessage = "Đơn vị cơ sở (tồn kho) không được để trống.")]
        [MaxLength(50)]
        public string BaseUnit { get; set; } = string.Empty; // VD: Hộp

        [MaxLength(50)]
        public string? PurchaseUnit { get; set; } // VD: Thùng

        [Range(1, 10000, ErrorMessage = "Tỷ lệ quy đổi phải lớn hơn hoặc bằng 1.")]
        public int ConversionRate { get; set; } = 1; // 1 Thùng = ? Hộp

        // --- Giá vốn ---
        [Range(0, 1000000000, ErrorMessage = "Giá vốn nhập vào không hợp lệ.")]
        public decimal CostPerPurchaseUnit { get; set; } = 0; // Giá mua theo đơn vị nhập (Thùng)

        // --- Cảnh báo ---
        [Range(0, 1000000)]
        public int? MinStockAlert { get; set; }

        // Mặc định là Active
        public bool IsActive { get; set; } = true;
    }
}