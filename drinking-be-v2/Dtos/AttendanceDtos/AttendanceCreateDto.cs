// File: Dtos/AttendanceDtos/AttendanceCreateDto.cs
using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AttendanceDtos
{
    public class AttendanceCreateDto
    {
        [Required]
        public int StaffId { get; set; }

        [Required]
        public int StoreId { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Mặc định hôm nay

        // Nếu tạo tay (Manual), có thể điền luôn giờ vào/ra
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public AttendanceStatusEnum Status { get; set; } = AttendanceStatusEnum.Present;

        public string? Note { get; set; }
    }
}