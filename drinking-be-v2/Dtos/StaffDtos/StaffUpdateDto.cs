using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.StaffDtos
{
    public class StaffUpdateDto
    {
        // Điều chuyển công tác
        public int? StoreId { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        // Thăng chức/Đổi vị trí
        public StaffPositionEnum? Position { get; set; }

        [MaxLength(20)]
        public string? CitizenId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
        public DateTime? HireDate { get; set; }

        // --- Cập nhật Lương ---
        public SalaryTypeEnum? SalaryType { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? HourlySalary { get; set; }
        public decimal? OvertimeHourlySalary { get; set; }

        // --- Cập nhật Ràng buộc ---
        public double? MinWorkHoursPerMonth { get; set; }
        public double? MaxWorkHoursPerMonth { get; set; }
        public double? MaxOvertimeHoursPerMonth { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}