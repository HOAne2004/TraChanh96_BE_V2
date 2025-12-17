// File: Dtos/SocialMediaDtos/SocialMediaReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.SocialMediaDtos
{
    public class SocialMediaReadDto
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!; // Cần Include Brand

        public int? StoreId { get; set; }
        public string? StoreName { get; set; } // Cần Include Store

        public string PlatformName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string? IconUrl { get; set; }
        public byte? SortOrder { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}