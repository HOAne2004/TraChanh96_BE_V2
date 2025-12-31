using drinking_be.Dtos.Common;
using drinking_be.Enums;

namespace drinking_be.Dtos.NotificationDtos
{
    public class NotificationFilterDto : PagingRequest
    {
        public NotificationTypeEnum? Type { get; set; }
        public string? Keyword { get; set; } // Tìm theo Title/Content
    }
}