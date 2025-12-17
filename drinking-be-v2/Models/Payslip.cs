using drinking_be.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Payslip
{
    public long Id { get; set; }

    // --- ĐỊNH DANH ---
    public int StaffId { get; set; }

    // Kỳ lương (Ví dụ: Tháng 11 Năm 2025)
    public int Month { get; set; }
    public int Year { get; set; }

    // Khoảng thời gian tính lương (VD: 01/11 - 30/11)
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }

    // --- SNAPSHOT CẤU HÌNH LƯƠNG (Lưu chết tại thời điểm tính) ---
    // Loại lương lúc tính (FullTime/PartTime)
    public SalaryTypeEnum AppliedSalaryType { get; set; }

    // Mức lương áp dụng
    public decimal AppliedBaseSalary { get; set; } = 0;
    public decimal AppliedHourlyRate { get; set; } = 0;
    public decimal AppliedOvertimeRate { get; set; } = 0;

    // --- TỔNG HỢP CÔNG (Từ bảng Attendance) ---
    public double TotalWorkHours { get; set; } = 0;     // Tổng giờ làm thường
    public double TotalOvertimeHours { get; set; } = 0; // Tổng giờ tăng ca
    public int TotalWorkDays { get; set; } = 0;         // Tổng số ngày đi làm

    // --- CHI TIẾT THU NHẬP ---
    public decimal SalaryBeforeTax { get; set; } = 0;   // Lương thô (Giờ * Rate hoặc Base)

    public decimal Allowance { get; set; } = 0;         // Phụ cấp (Ăn trưa, xăng xe...)
    public decimal Bonus { get; set; } = 0;             // Thưởng (Thủ công + Cộng dồn từ Attendance)
    public decimal Deduction { get; set; } = 0;         // Phạt (Thủ công + Cộng dồn từ Attendance)

    public decimal TaxAmount { get; set; } = 0;         // Thuế TNCN (Tính toán nếu có)

    // --- THỰC LĨNH ---
    // FinalSalary = SalaryBeforeTax + Allowance + Bonus - Deduction - TaxAmount
    public decimal FinalSalary { get; set; } = 0;

    // --- QUẢN TRỊ ---
    public PayslipStatusEnum Status { get; set; } = PayslipStatusEnum.Draft;
    [MaxLength(500)]
    public string? Note { get; set; } // Ghi chú của kế toán/Manager

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Property
    public virtual Staff Staff { get; set; } = null!;
}