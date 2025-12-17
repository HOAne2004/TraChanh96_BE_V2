// File: Dtos/PayslipDtos/PayslipReadDto.cs
using drinking_be.Enums;

namespace drinking_be.Dtos.PayslipDtos
{
    public class PayslipReadDto
    {
        public long Id { get; set; }

        public int StaffId { get; set; }
        public string StaffName { get; set; } = null!; // Cần Include Staff
        public string StaffPosition { get; set; } = null!;

        public int Month { get; set; }
        public int Year { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        // --- Thông tin tổng hợp ---
        public double TotalWorkHours { get; set; }
        public double TotalOvertimeHours { get; set; }
        public int TotalWorkDays { get; set; }

        // --- Chi tiết tiền ---
        public decimal SalaryBeforeTax { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalSalary { get; set; }

        public string Status { get; set; } = null!; // Enum String
        public string? Note { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}