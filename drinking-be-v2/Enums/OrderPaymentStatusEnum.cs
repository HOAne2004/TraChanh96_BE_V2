// File: Enums/OrderPaymentStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum OrderPaymentStatusEnum : short
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