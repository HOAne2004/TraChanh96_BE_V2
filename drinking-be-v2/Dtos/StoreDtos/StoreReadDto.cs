// File: Dtos/StoreDtos/StoreReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.AddressDtos;
using drinking_be.Dtos.SocialMediaDtos;
using drinking_be.Dtos.ShopTableDtos;

namespace drinking_be.Dtos.StoreDtos
{
    public class StoreReadDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!; // Cần Include Brand

        public string? ImageUrl { get; set; }

        public DateTime? OpenDate { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public decimal? ShippingFeeFixed { get; set; }
        public decimal? ShippingFeePerKm { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;
        public byte? SortOrder { get; set; }
        public bool? MapVerified { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ⭐ QUAN HỆ 1:1: Địa chỉ chi tiết (Cần Include Address)
        public AddressReadDto Address { get; set; } = null!;

        // Các Navigation Properties khác
        public ICollection<SocialMediaReadDto> SocialMedias { get; set; } = new List<SocialMediaReadDto>();
        public ICollection<ShopTableReadDto> ShopTables { get; set; } = new List<ShopTableReadDto>();
    }
}