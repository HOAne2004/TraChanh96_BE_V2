// File: Dtos/BrandDtos/BrandReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.SocialMediaDtos;
using drinking_be.Dtos.PolicyDtos;
// Giả định bạn có SocialMediaReadDto và PolicyReadDto

namespace drinking_be.Dtos.BrandDtos
{
    public class BrandReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public string? LogoUrl { get; set; }
        public string? Address { get; set; }
        public string? Hotline { get; set; }
        public string? EmailSupport { get; set; }
        public string? TaxCode { get; set; }
        public string? Slogan { get; set; }
        public string? CopyrightText { get; set; }

        public DateTime? EstablishedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        // Navigation Properties (Đã map sang Read DTOs tương ứng)
        public ICollection<PolicyReadDto> Policies { get; set; } = new List<PolicyReadDto>();
        public ICollection<SocialMediaReadDto> SocialMedias { get; set; } = new List<SocialMediaReadDto>();
    }
}