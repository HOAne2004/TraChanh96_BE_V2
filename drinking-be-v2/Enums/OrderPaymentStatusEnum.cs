// File: Enums/OrderPaymentStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum OrderPaymentStatusEnum : byte
    {
        [Description("Chờ thanh toán")]
        Pending = 1,

        [Description("Đã thanh toán")]
        Paid = 2,

        [Description("Đã hoàn tiền")]
        Refunded = 3,

        [Description("Thanh toán thất bại")]
        Failed = 4
    }
}