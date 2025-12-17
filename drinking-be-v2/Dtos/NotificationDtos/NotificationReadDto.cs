using drinking_be.Enums;

namespace drinking_be.Dtos.NotificationDtos
{
    public class NotificationReadDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        // Trả về String cho FE dễ hiển thị (hoặc trả Int tùy quy ước)
        public string Type { get; set; } = null!;
        public string? ReferenceId { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}