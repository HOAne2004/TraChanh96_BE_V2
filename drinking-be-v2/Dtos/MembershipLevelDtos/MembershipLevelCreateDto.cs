// File: Dtos/MembershipLevelDtos/MembershipLevelCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.MembershipLevelDtos
{
    public class MembershipLevelCreateDto
    {
        [Required(ErrorMessage = "Tên cấp độ không được để trống.")]
        [MaxLength(35)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chi tiêu tối thiểu không được để trống.")]
        [Range(0, 1000000000, ErrorMessage = "Giá trị chi tiêu tối thiểu không hợp lệ.")]
        public decimal MinSpendRequired { get; set; }

        [Required(ErrorMessage = "Thời hạn duy trì cấp độ (ngày) không được để trống.")]
        [Range(1, 1000, ErrorMessage = "Thời hạn phải lớn hơn 0.")]
        public short DurationDays { get; set; }

        public string? Benefits { get; set; }

        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}