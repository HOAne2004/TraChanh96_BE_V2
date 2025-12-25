using drinking_be.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Attendance
{
    public long Id { get; set; } // Dữ liệu nhiều, dùng long

    // --- LIÊN KẾT ---
    public int StaffId { get; set; }
    public int StoreId { get; set; } // Lưu Store để biết nhân viên làm việc ở đâu hôm nay

    // --- THỜI GIAN ---
    public DateOnly Date { get; set; } // Ngày làm việc (VD: 2023-10-25)

    public DateTime? CheckInTime { get; set; } // Giờ vào thực tế
    public DateTime? CheckOutTime { get; set; } // Giờ ra thực tế

    // Các trường tính toán (Lưu lại để đỡ phải tính mỗi lần query)
    public double WorkingHours { get; set; } = 0; // Tổng giờ làm thường
    public double OvertimeHours { get; set; } = 0; // Tổng giờ tăng ca

    // --- TRẠNG THÁI ---
    public AttendanceStatusEnum? Status { get; set; } // Mặc định là Vắng nếu chưa check-in

    [MaxLength(500)]
    public string? Note { get; set; } // Ghi chú (VD: "Hỏng xe đến muộn")

    // --- TÀI CHÍNH HÀNG NGÀY ---
    public decimal DailyBonus { get; set; } = 0;     // Thưởng nóng trong ngày
    public decimal DailyDeduction { get; set; } = 0; // Phạt vi phạm trong ngày

    // --- QUẢN TRỊ ---
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Staff Staff { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
}