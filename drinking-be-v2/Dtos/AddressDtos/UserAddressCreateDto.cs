using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AddressDtos
{
    public class UserAddressCreateDto
    {
        // === USER CONTEXT ===
        [Required]
        public int UserId { get; set; }

        // === CONTACT INFO (BẮT BUỘC) ===
        [Required(ErrorMessage = "Tên người nhận là bắt buộc.")]
        [MaxLength(50)]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b")]
        public string RecipientPhone { get; set; } = string.Empty;

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

        public bool IsDefault { get; set; } = false;
    }
}
