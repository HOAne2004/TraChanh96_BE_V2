// File: Dtos/UserDtos/UserRegisterDto.cs

using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.UserDtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Email không đúng định dạng (Ví dụ: user@gmail.com, không chứa dấu cách).")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [RegularExpression(@"^(0|\+84)(3|5|7|8|9)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ (Phải đúng chuẩn Việt Nam, VD: 0912345678).")]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}