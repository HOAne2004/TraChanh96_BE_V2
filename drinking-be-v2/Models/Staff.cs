using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Staff : ISoftDelete
{
    public int Id { get; set; }

    // Dùng PublicId để bảo mật khi gọi API
    public Guid PublicId { get; set; }

    // --- LIÊN KẾT ---
    public int UserId { get; set; } // Khóa ngoại 1-1 với User (Bắt buộc)
    public int? StoreId { get; set; } // Nhân viên thuộc cửa hàng nào (Null nếu là nhân viên HQ)

    // --- THÔNG TIN CÁ NHÂN ---
    public string FullName { get; set; } = null!;

    public string? CitizenId { get; set; } // Căn cước công dân

    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; } // Địa chỉ thường trú

    // --- CÔNG VIỆC ---
    public StaffPositionEnum Position { get; set; } = StaffPositionEnum.Server;

    public DateTime HireDate { get; set; } // Ngày vào làm

    // --- CẤU HÌNH LƯƠNG & RÀNG BUỘC (Lưu cấu hình, không phải bảng lương tháng) ---
    public SalaryTypeEnum SalaryType { get; set; } = SalaryTypeEnum.PartTime;

    // Dùng cho FullTime
    public decimal? BaseSalary { get; set; }

    // Dùng cho PartTime (hoặc tính OT cho FullTime nếu cần)
    public decimal? HourlySalary { get; set; }

    // Mức lương tăng ca (vnđ/giờ)
    public decimal? OvertimeHourlySalary { get; set; }

    // Ràng buộc KPI / Chấm công (số giờ)
    public double? MinWorkHoursPerMonth { get; set; } // Ví dụ: 20 giờ
    public double? MaxWorkHoursPerMonth { get; set; } // Ví dụ: 200 giờ
    public double? MaxOvertimeHoursPerMonth { get; set; }

    // --- QUẢN TRỊ ---
    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; } // Xóa mềm (Nghỉ việc)

    // --- NAVIGATION PROPERTIES ---
    public virtual User User { get; set; } = null!;
    public virtual Store? Store { get; set; }
}