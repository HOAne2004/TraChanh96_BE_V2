// File: Dtos/UserDtos/UserUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        [MaxLength(50)]
        public string? Username { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? ThumbnailUrl { get; set; }

        // Các trường quản trị (Admin only)
        public UserRoleEnum? RoleId { get; set; }
        public UserStatusEnum? Status { get; set; }
        public bool? EmailVerified { get; set; }
        public int? CurrentCoins { get; set; }
    }
}