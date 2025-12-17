// File: Dtos/PayslipDtos/PayslipUpdateDto.cs
using drinking_be.Enums;

namespace drinking_be.Dtos.PayslipDtos
{
    public class PayslipUpdateDto
    {
        // Điều chỉnh tiền nong (Cộng dồn vào số đã tính)
        public decimal? Bonus { get; set; }
        public decimal? Deduction { get; set; }
        public decimal? Allowance { get; set; }

        // Cập nhật trạng thái (VD: Từ Draft -> Confirmed -> Paid)
        public PayslipStatusEnum? Status { get; set; }

        public string? Note { get; set; }
    }
}