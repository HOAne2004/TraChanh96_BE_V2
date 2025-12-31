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
        [Required(ErrorMessage = "Tên người nhận là bắt buộc.")]
        [MaxLength(50, ErrorMessage = "Tên không quá 50 ký tự.")]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵýỷỹ\s]+$",
            ErrorMessage = "Tên người nhận không được chứa số hoặc ký tự đặc biệt.")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b",
            ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam.")]
        public string RecipientPhone { get; set; } = string.Empty;

        // --- ĐỊA CHỈ CHI TIẾT VÀ VÙNG ---

        [Required(ErrorMessage = "Địa chỉ chi tiết là bắt buộc.")]
        [MaxLength(200, ErrorMessage = "Địa chỉ không quá 200 ký tự.")]
        [RegularExpression(@"^[^<>{}$]+$", ErrorMessage = "Địa chỉ chứa ký tự không hợp lệ.")]
        public string AddressDetail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ đầy đủ (FullAddress) không được để trống.")]
        [MaxLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tỉnh/Thành phố không được để trống.")]
        [MaxLength(50)]
        public string Province { get; set; } = string.Empty;

        [MaxLength(50)]
        [Required(ErrorMessage = "Quận/Huyện là bắt buộc.")]
        public string? District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xã/Phường không được để trống.")]
        [MaxLength(50)]
        public string Commune { get; set; } = string.Empty;

        // --- TỌA ĐỘ ---
        [Range(8.0, 24.0, ErrorMessage = "Vĩ độ không hợp lệ (Phải nằm trong lãnh thổ VN).")]
        public double Latitude { get; set; }

        [Range(102.0, 110.0, ErrorMessage = "Kinh độ không hợp lệ (Phải nằm trong lãnh thổ VN).")]
        public double Longitude { get; set; }

        // --- QUẢN LÝ ---
        public bool IsDefault { get; set; } = false;

        // Trạng thái (Chỉ Admin mới có thể thay đổi Status, mặc định là Active)
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}