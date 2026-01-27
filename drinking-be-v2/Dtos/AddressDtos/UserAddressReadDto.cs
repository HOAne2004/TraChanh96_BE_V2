// File: Dtos/AddressDtos/AddressReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.AddressDtos
{
    public class UserAddressReadDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }

        public string? RecipientName { get; set; } = null!;
        public string? RecipientPhone { get; set; } = null!;

        public string FullAddress { get; set; } = null!;
        public string AddressDetail { get; set; } = null!;

        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Commune { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsDefault { get; set; }

        // Trạng thái dưới dạng string/label cho dễ đọc
        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}