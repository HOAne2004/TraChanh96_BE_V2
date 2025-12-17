// File: Dtos/VoucherDtos/UserVoucherCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.VoucherDtos
{
    public class UserVoucherCreateDto
    {
        [Required(ErrorMessage = "Mã User không được để trống.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Mã Mẫu Voucher (Template) không được để trống.")]
        public int VoucherTemplateId { get; set; }

        // Mã Voucher (Nếu không được cung cấp, Service Layer sẽ tạo ngẫu nhiên)
        [MaxLength(20)]
        public string? VoucherCode { get; set; }

        // Ngày hết hạn (Nếu không được cung cấp, Service Layer sẽ lấy từ Template)
        public DateTime? ExpiryDate { get; set; }

        // Status, IssuedDate, UsedDate, OrderIdUsed sẽ được Service Layer gán
    }
}