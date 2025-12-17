using drinking_be.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Models
{
    public class Notification
    {
        public long Id { get; set; }

        // UserId = null => Gửi tất cả (Broadcast)
        public int? UserId { get; set; }

        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        // ⭐ Cập nhật: Dùng Enum
        public NotificationTypeEnum Type { get; set; } = NotificationTypeEnum.System;

        public string? ReferenceId { get; set; } // Mã đơn hàng / Mã Voucher

        public bool IsRead { get; set; } = false;

        // ⭐ THÊM MỚI: Thời gian dự kiến gửi
        // Nếu null => Gửi ngay lập tức (Instant)
        // Nếu có giá trị => Chờ đến giờ đó mới hiện
        public DateTime? ScheduledTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
    }
}