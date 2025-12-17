using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.NotificationDtos
{
    public class NotificationCreateDto
    {
        public int? UserId { get; set; } // Null = Gửi tất cả

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public NotificationTypeEnum Type { get; set; } = NotificationTypeEnum.System;

        public string? ReferenceId { get; set; }

        // Nếu muốn hẹn giờ thì truyền vào, không thì để null (gửi ngay)
        public DateTime? ScheduledTime { get; set; }
    }
}