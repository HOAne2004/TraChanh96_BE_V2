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
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}