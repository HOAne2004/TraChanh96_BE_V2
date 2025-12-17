using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.VoucherDtos
{
    public class VoucherApplyDto
    {
        [Required]
        public string VoucherCode { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal OrderTotalAmount { get; set; } // Tổng tiền hàng để tính điều kiện
    }
}