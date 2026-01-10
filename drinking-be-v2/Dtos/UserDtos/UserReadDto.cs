// File: Dtos/UserDtos/UserReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.MembershipDtos;

namespace drinking_be.Dtos.UserDtos
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? ThumbnailUrl { get; set; }

        // --- Trạng thái & Vai trò ---
        public UserRoleEnum Role { get; set; }
        public string RoleLabel { get; set; } = null!; // Vai trò (String/Label)
        public UserStatusEnum Status { get; set; }
        public string StatusLabel { get; set; } = null!; // Trạng thái (String/Label)

        public int? CurrentCoins { get; set; }
        public bool? EmailVerified { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Thông tin Thành viên (Cần Include Membership)
        public MembershipReadDto? Membership { get; set; }
    }
}