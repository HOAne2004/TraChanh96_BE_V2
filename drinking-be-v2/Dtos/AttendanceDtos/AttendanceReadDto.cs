// File: Dtos/AttendanceDtos/AttendanceReadDto.cs
using drinking_be.Enums;

namespace drinking_be.Dtos.AttendanceDtos
{
    public class AttendanceReadDto
    {
        public long Id { get; set; }

        public int StaffId { get; set; }
        public string StaffName { get; set; } = null!; // Cần Include Staff

        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!; // Cần Include Store

        public DateOnly Date { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public double WorkingHours { get; set; }
        public double OvertimeHours { get; set; }

        public string Status { get; set; } = null!; // Enum String
        public string? Note { get; set; }

        public decimal DailyBonus { get; set; }
        public decimal DailyDeduction { get; set; }
    }
}