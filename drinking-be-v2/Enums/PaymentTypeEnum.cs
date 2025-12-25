// File: Enums/PaymentTypeEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum PaymentTypeEnum : short
    {
        [Description("Thanh toán khi nhận hàng")]
        COD = 1,

        [Description("Chuyển khoản Ngân hàng")]
        BankTransfer = 2,

        [Description("Ví điện tử")]
        EWallet = 3,

        [Description("Thẻ tín dụng/ghi nợ")]
        Card = 4
    }
}