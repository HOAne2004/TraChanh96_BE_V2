using drinking_be.Enums;

namespace drinking_be.Dtos.StaffDtos
{
    public class StaffReadDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; } // Quan trọng cho API

        // --- Thông tin Tài khoản (Link) ---
        public int UserId { get; set; }
        public string UserName { get; set; } = null!; // Cần Include User
        public string Email { get; set; } = null!;    // Cần Include User
        public string? UserAvatar { get; set; }       // Cần Include User

        // --- Thông tin Cửa hàng ---
        public int? StoreId { get; set; }
        public string? StoreName { get; set; } // Cần Include Store

        // --- Thông tin Nhân sự ---
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!; // String Label
        public string? CitizenId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime HireDate { get; set; }

        // --- Lương & Cấu hình ---
        public string SalaryType { get; set; } = null!; // String Label
        public decimal? BaseSalary { get; set; }
        public decimal? HourlySalary { get; set; }
        public decimal? OvertimeHourlySalary { get; set; }

        public double? MinWorkHoursPerMonth { get; set; }
        public double? MaxWorkHoursPerMonth { get; set; }

        // Trạng thái
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}