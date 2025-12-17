// File: Dtos/BrandDtos/BrandCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.BrandDtos
{
    public class BrandCreateDto
    {
        [Required(ErrorMessage = "Tên thương hiệu không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên công ty không được để trống.")]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        // Thông tin liên hệ và địa chỉ
        public string? LogoUrl { get; set; }
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Hotline không đúng định dạng.")]
        public string? Hotline { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string? EmailSupport { get; set; }

        public string? TaxCode { get; set; }
        public string? Slogan { get; set; }
        public string? CopyrightText { get; set; }

        public DateTime? EstablishedDate { get; set; }

        // Mặc định là Active
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}