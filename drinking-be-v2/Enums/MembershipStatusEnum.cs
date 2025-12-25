// File: Enums/MembershipStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum MembershipStatusEnum : short
    {
        // Trạng thái chuẩn

        [Description("Đang hoạt động")]
        Active = 1, // Tư cách thành viên hiện tại có hiệu lực

        [Description("Đã hết hạn")]
        Expired = 2, // Tư cách thành viên đã hết thời gian hiệu lực

        [Description("Tạm thời bị đình chỉ")]
        Suspended = 3, // Tư cách thành viên bị tạm đình chỉ (ví dụ: do vi phạm chính sách)

        [Description("Đã nâng cấp")]
        Upgraded = 4, // Đã chuyển sang cấp độ thành viên cao hơn (thường chỉ dùng cho lịch sử giao dịch)

        // Trạng thái xóa mềm (Soft Delete)
        [Description("Đã xóa")]
        Deleted = 99 // Dùng cho Soft Delete, ví dụ khi tài khoản người dùng bị xóa mềm
    }
} 