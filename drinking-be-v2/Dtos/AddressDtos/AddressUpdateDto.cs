// File: Dtos/AddressDtos/AddressUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AddressDtos
{
    public class AddressUpdateDto
    {
        // Có thể update thông tin người nhận
        [MaxLength(100)]
        public string? RecipientName { get; set; }

        [Phone]
        public string? RecipientPhone { get; set; }

        // Có thể update địa chỉ chi tiết/tọa độ
        [MaxLength(255)]
        public string? AddressDetail { get; set; }

        [MaxLength(500)]
        public string? FullAddress { get; set; }

        [MaxLength(50)]
        public string? Province { get; set; }

        [MaxLength(50)]
        public string? District { get; set; }

        [MaxLength(50)]
        public string? Commune { get; set; }

        [Range(-90.0, 90.0)]
        public double? Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public double? Longitude { get; set; }

        // Chỉ Admin/Service mới thay đổi Status
        public PublicStatusEnum? Status { get; set; }

        // Có thể thay đổi trạng thái mặc định
        public bool? IsDefault { get; set; }
    }
}