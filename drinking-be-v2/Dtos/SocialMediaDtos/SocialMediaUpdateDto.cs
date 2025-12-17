// File: Dtos/SocialMediaDtos/SocialMediaUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.SocialMediaDtos
{
    public class SocialMediaUpdateDto
    {
        // Có thể thay đổi StoreId liên kết (chuyển từ Store này sang Store khác)
        public int? StoreId { get; set; }

        [MaxLength(30)]
        public string? PlatformName { get; set; }

        [Url]
        [MaxLength(500)]
        public string? Url { get; set; }

        public string? IconUrl { get; set; }

        public byte? SortOrder { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}