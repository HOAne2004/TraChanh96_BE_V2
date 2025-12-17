// File: Enums/UserVoucherStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum UserVoucherStatusEnum : byte
    {
        [Description("Chưa sử dụng")]
        Unused = 1,

        [Description("Đã sử dụng")]
        Used = 2,

        [Description("Đã hết hạn")]
        Expired = 3,

        [Description("Đã bị xóa")]
        Deleted = 99,
    }
}