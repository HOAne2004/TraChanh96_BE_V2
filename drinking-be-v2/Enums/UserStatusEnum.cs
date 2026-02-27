// File: Enums/UserStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum UserStatusEnum : short
    {
        // Trạng thái hoạt động (Dùng lại giá trị số của PublicStatusEnum)

        [Description("Đang hoạt động")]
        Active = 2,

        // Trạng thái hạn chế

        [Description("Đã bị khóa")]
        Locked = 3,

        // Trạng thái xóa mềm (Dùng lại giá trị số của PublicStatusEnum)

        [Description("Đã bị xóa")]
        Deleted = 99
    }
}