// File: Dtos/AddressDtos/AddressCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.AddressDtos
{
    public class AddressCreateDto
    {
        // ⭐ UserId chỉ cần cho User Address (nullable vì có thể là Store Address)
        public int? UserId { get; set; }

        // --- THÔNG TIN LIÊN HỆ ---
        [Required(ErrorMessage = "Tên người nhận không được để trống.")]
        [MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string RecipientPhone { get; set; } = string.Empty;

        // --- ĐỊA CHỈ CHI TIẾT VÀ VÙNG ---

        [Required(ErrorMessage = "Địa chỉ chi tiết (số nhà, tên đường) không được để trống.")]
        [MaxLength(255)]
        public string AddressDetail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ đầy đủ (FullAddress) không được để trống.")]
        [MaxLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tỉnh/Thành phố không được để trống.")]
        [MaxLength(50)]
        public string Province { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xã/Phường không được để trống.")]
        [MaxLength(50)]
        public string Commune { get; set; } = string.Empty;

        // --- TỌA ĐỘ ---
        [Required(ErrorMessage = "Vĩ độ (Latitude) không được để trống.")]
        [Range(-90.0, 90.0, ErrorMessage = "Vĩ độ không hợp lệ.")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Kinh độ (Longitude) không được để trống.")]
        [Range(-180.0, 180.0, ErrorMessage = "Kinh độ không hợp lệ.")]
        public double Longitude { get; set; }

        // --- QUẢN LÝ ---
        public bool IsDefault { get; set; } = false;

        // Trạng thái (Chỉ Admin mới có thể thay đổi Status, mặc định là Active)
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}