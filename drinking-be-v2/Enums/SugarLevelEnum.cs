// File: Enums/SugarLevelEnum.cs (Ví dụ)
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum SugarLevelEnum : byte
    {
        [Description("Không đường")]
        S0 = 0,
        [Description("30% Đường")]
        S30 = 30,
        [Description("50% Đường")]
        S50 = 50,
        [Description("70% Đường")]
        S70 = 70,
        [Description("100% Đường")]
        S100 = 100
        // Nếu cần thêm tùy chọn khác, bạn có thể thêm:
        // [Description("Tùy chỉnh")]
        // Custom = 255
    }
}