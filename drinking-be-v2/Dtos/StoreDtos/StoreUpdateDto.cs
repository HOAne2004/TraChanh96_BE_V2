// File: Dtos/StoreDtos/StoreUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.StoreDtos
{
    public class StoreUpdateDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        // Có thể thay đổi Slug
        public string? Slug { get; set; }

        // Nếu thay đổi, địa chỉ mới phải tồn tại
        public long? AddressId { get; set; }

        public string? ImageUrl { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public DateTime? OpenDate { get; set; }

        public decimal? ShippingFeeFixed { get; set; }
        public decimal? ShippingFeePerKm { get; set; }

        public StoreStatusEnum? Status { get; set; }

        public byte? SortOrder { get; set; }
        public bool? MapVerified { get; set; }
    }
}