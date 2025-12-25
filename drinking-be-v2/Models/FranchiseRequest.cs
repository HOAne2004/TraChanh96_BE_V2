using drinking_be.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class FranchiseRequest
{
    public int Id { get; set; }

    // --- Thông tin Ứng viên (Candidate) ---
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Address { get; set; } // Địa chỉ hiện tại của ứng viên

    // --- Thông tin Dự án Nhượng quyền ---
    public string TargetArea { get; set; } = null!; // Khu vực dự định mở (VD: Cầu Giấy, HN)
    public decimal? EstimatedBudget { get; set; } // Ngân sách dự kiến đầu tư
    public string? ExperienceDescription { get; set; } // Kinh nghiệm kinh doanh trước đây

    // --- Quản lý Nội bộ (Admin) ---
    public FranchiseStatusEnum? Status { get; set; }
    public string? AdminNote { get; set; } // Ghi chú nội bộ (VD: "Khách có mặt bằng đẹp", "Gọi lại sau 5h")

    // Ai là người phụ trách hồ sơ này? (Liên kết với User - Role Admin/Manager)
    public int? ReviewerId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation Property
    [ForeignKey("ReviewerId")]
    public virtual User? Reviewer { get; set; }
}