// File: Enums/PolicyReviewStatusEnum.cs
using System.ComponentModel; // ⭐ CẦN THIẾT

namespace drinking_be.Enums
{
    public enum PolicyReviewStatusEnum : byte
    {
        [Description("Chờ phê duyệt")]
        Pending = 1,

        [Description("Đã phê duyệt")]
        Approved = 2,

        [Description("Từ chối")]
        Rejected = 3,

        [Description("Đã hủy")]
        Cancelled = 4,

        [Description("Đã bị xóa")]
        Deleted = 99

    }
}