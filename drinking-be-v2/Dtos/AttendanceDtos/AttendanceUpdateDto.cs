// File: Dtos/AttendanceDtos/AttendanceUpdateDto.cs
using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AttendanceDtos
{
    public class AttendanceUpdateDto
    {
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        // Manager có thể sửa giờ công tay
        public double? WorkingHours { get; set; }
        public double? OvertimeHours { get; set; }

        public AttendanceStatusEnum? Status { get; set; }
        public string? Note { get; set; }

        public decimal? DailyBonus { get; set; }
        public decimal? DailyDeduction { get; set; }
    }
}