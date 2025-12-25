using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum SalaryTypeEnum : short
    {
        [Description("Toàn thời gian (Lương cứng)")]
        FullTime = 1,

        [Description("Bán thời gian (Lương giờ)")]
        PartTime = 2
    }
}