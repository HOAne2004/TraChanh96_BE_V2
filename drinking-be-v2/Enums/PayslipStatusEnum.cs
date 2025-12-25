using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum PayslipStatusEnum : short
    {
        [Description("Nháp (Đang tính toán)")]
        Draft = 1,

        [Description("Đã chốt (Chờ thanh toán)")]
        Confirmed = 2,

        [Description("Đã thanh toán")]
        Paid = 3,

        [Description("Đã hủy")]
        Cancelled = 4
    }
}