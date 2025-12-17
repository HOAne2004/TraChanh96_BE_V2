using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum AttendanceStatusEnum : byte
    {
        [Description("Đi làm đúng giờ")]
        Present = 1,

        [Description("Đi muộn")]
        Late = 2,

        [Description("Về sớm")]
        LeaveEarly = 3,

        [Description("Nghỉ phép (Có lương)")]
        PaidLeave = 4,

        [Description("Nghỉ không phép (Vắng)")]
        Absent = 5,

        [Description("Ngày nghỉ lễ")]
        Holiday = 6
    }
}