// File: Dtos/BrandDtos/BrandUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.BrandDtos
{
    public class BrandUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(150)]
        public string? CompanyName { get; set; }

        public string? LogoUrl { get; set; }
        public string? Address { get; set; }

        [Phone]
        public string? Hotline { get; set; }

        [EmailAddress]
        public string? EmailSupport { get; set; }

        public string? TaxCode { get; set; }
        public string? Slogan { get; set; }
        public string? CopyrightText { get; set; }

        public DateTime? EstablishedDate { get; set; }

        // Admin có thể thay đổi trạng thái
        public PublicStatusEnum? Status { get; set; }
    }
}