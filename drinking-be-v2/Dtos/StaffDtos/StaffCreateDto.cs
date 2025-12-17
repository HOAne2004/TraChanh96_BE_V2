using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.StaffDtos
{
    public class StaffCreateDto
    {
        [Required(ErrorMessage = "Mã tài khoản User không được để trống.")]
        public int UserId { get; set; }

        // Null nếu là nhân viên văn phòng/HQ
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vị trí công việc không được để trống.")]
        public StaffPositionEnum Position { get; set; }

        // --- Thông tin cá nhân ---
        [MaxLength(20)]
        public string? CitizenId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        // --- Cấu hình Lương ---
        [Required(ErrorMessage = "Loại lương không được để trống.")]
        public SalaryTypeEnum SalaryType { get; set; }

        [Range(0, 1000000000)]
        public decimal? BaseSalary { get; set; }

        [Range(0, 10000000)]
        public decimal? HourlySalary { get; set; }

        [Range(0, 10000000)]
        public decimal? OvertimeHourlySalary { get; set; }

        // --- Ràng buộc (KPI) ---
        public double? MinWorkHoursPerMonth { get; set; }
        public double? MaxWorkHoursPerMonth { get; set; }
        public double? MaxOvertimeHoursPerMonth { get; set; }

        // Trạng thái mặc định là Active, không cần truyền
    }
}