using System.ComponentModel.DataAnnotations;

namespace drinking_be.Models
{
    public class SystemSetting
    {
        [Key]
        [MaxLength(50)]
        public string Key { get; set; } = null!; // Ví dụ: "MaxQuantityPerItem", "OrderAutoCancelMinutes"

        [MaxLength(500)]
        public string Value { get; set; } = null!; // Lưu dưới dạng chuỗi, khi dùng sẽ ép kiểu (Parse)

        [MaxLength(20)]
        public string DataType { get; set; } = "string";

        [MaxLength(200)]
        public string? Description { get; set; } // Ghi chú cho Admin hiểu trên giao diện
    }
}
