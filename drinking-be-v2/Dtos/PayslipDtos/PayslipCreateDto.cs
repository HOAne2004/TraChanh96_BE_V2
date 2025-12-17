// File: Dtos/PayslipDtos/PayslipCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.PayslipDtos
{
    public class PayslipCreateDto
    {
        [Required]
        public int StaffId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2020, 2100)]
        public int Year { get; set; }

        // Có thể truyền ngày chốt công tùy chỉnh (nếu không mặc định là ngày 1 đến cuối tháng)
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}