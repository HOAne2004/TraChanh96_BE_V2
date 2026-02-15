using drinking_be.Dtos.Common;
using drinking_be.Enums;

namespace drinking_be.Dtos.UserDtos
{
    public class UserFilterDto: PagingRequest
    {
        public string? Keyword { get; set; }
        public UserStatusEnum? Status { get; set; } 
        public UserRoleEnum? RoleId { get; set; }
        public int? MembershipLevelId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
