// File: Dtos/MembershipDtos/MembershipCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.MembershipDtos
{
    public class MembershipCreateDto
    {
        // UserId được Service Layer gán
        [Required(ErrorMessage = "Mã người dùng không được để trống.")]
        public int UserId { get; set; } 

        [Required(ErrorMessage = "Mã cấp độ (LevelId) không được để trống.")]
        public byte LevelId { get; set; }

        // CardCode thường được tạo tự động
        public string? CardCode { get; set; }

        // Thời gian hiệu lực ban đầu
        public DateOnly LevelEndDate { get; set; }

        public decimal? TotalSpent { get; set; } = 0m;

        // Trạng thái ban đầu
        public MembershipStatusEnum Status { get; set; } = MembershipStatusEnum.Active;
    }
}