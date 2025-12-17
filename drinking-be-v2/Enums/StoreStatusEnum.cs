// File: Enums/StoreStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum StoreStatusEnum : byte
    {
        [Description("Chờ ngày khai trương")]
        ComingSoon = 1,

        [Description("Đang hoạt động")]
        Active = 2, 

        [Description("Tạm thời đóng cửa")]
        TemporarilyClosed = 3,

        [Description("Đã đóng cửa vĩnh viễn")]
        PermanentlyClosed = 4,

        [Description("Đã bị xóa")]
        Deleted = 99
    }
}