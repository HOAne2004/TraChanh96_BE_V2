// File: Enums/IceLevelEnum.cs (Ví dụ)
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum IceLevelEnum : short
    {
        // Tùy chọn đặc biệt (Non-numeric)
        [Description("Không đá")]
        None = 1,
        [Description("Ấm")]
        Warm = 2,
        [Description("Nóng")]
        Hot = 3,

        // Tùy chọn phần trăm
        [Description("30% Đá")]
        I30 = 30,
        [Description("50% Đá")]
        I50 = 50,
        [Description("70% Đá")]
        I70 = 70,
        [Description("100% Đá")]
        I100 = 100
    }
}