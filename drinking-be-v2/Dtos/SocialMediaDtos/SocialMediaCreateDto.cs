// File: Dtos/SocialMediaDtos/SocialMediaCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.SocialMediaDtos
{
    public class SocialMediaCreateDto
    {
        [Required(ErrorMessage = "Mã Brand không được để trống.")]
        public int BrandId { get; set; }

        // Liên kết với Store là tùy chọn
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "Tên nền tảng (ví dụ: Facebook, Instagram) không được để trống.")]
        [MaxLength(30)]
        public string PlatformName { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL không được để trống.")]
        [Url(ErrorMessage = "URL không đúng định dạng.")]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public string? IconUrl { get; set; }

        public byte? SortOrder { get; set; }

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}