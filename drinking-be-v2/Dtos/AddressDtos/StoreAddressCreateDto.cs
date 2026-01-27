using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AddressDtos
{
    public class StoreAddressCreateDto
    {
        // === STORE CONTEXT ===
        [Required]
        public int StoreId { get; set; }

        // === ADDRESS ===
        [Required]
        [MaxLength(200)]
        public string AddressDetail { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Province { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string District { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Commune { get; set; } = string.Empty;

        // === LOCATION ===
        [Range(8, 24)]
        public double Latitude { get; set; }

        [Range(102, 110)]
        public double Longitude { get; set; }

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}
