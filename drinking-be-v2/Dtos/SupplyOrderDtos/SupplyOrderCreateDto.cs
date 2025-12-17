using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.SupplyOrderDtos
{
    public class SupplyOrderCreateDto
    {
        // StoreId: Manager nhập cho cửa hàng nào?
        // (Nếu null => Admin nhập cho Kho tổng)
        public int? StoreId { get; set; }

        public string? Note { get; set; } // Ví dụ: "Cần gấp trước thứ 7"

        public DateTime? ExpectedDeliveryDate { get; set; }

        // Danh sách các nguyên liệu cần nhập
        [Required]
        [MinLength(1, ErrorMessage = "Phiếu nhập phải có ít nhất 1 nguyên liệu.")]
        public List<SupplyOrderItemCreateDto> Items { get; set; } = new List<SupplyOrderItemCreateDto>();
    }

    // DTO Con: Chi tiết món nhập
    public class SupplyOrderItemCreateDto
    {
        [Required]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }
}