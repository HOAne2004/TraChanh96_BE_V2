// File: Enums/UserRoleEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum UserRoleEnum : short
    {
        [Description("Khách hàng")]
        Customer = 1,

        [Description("Quản trị viên")]
        Admin = 2,

        [Description("Quản lý cửa hàng")]
        Manager = 3,

        [Description("Nhân viên")]
        Staff = 4
    }
}