// File: Enums/ReservationStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum ReservationStatusEnum : short
    {
        [Description("Chờ xác nhận")]
        Pending = 1,

        [Description("Đã xác nhận")]
        Confirmed = 2,

        [Description("Khách đã đến")]
        Arrived = 3,

        [Description("Khách không đến")]
        NoShow = 4,

        [Description("Đã hủy")]
        Cancelled = 5,

        [Description("Hoàn thành")]
        Completed = 6,

        [Description("Đã bị xóa")]
        Deleted = 99
    }
}