using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum StaffPositionEnum : byte
    {
        [Description("Nhân viên Văn phòng/HQ")]
        OfficeStaff = 1,

        [Description("Tư vấn Nhượng quyền")]
        FranchiseConsultant = 2,

        [Description("Cửa hàng trưởng")]
        StoreManager = 10,

        [Description("Nhân viên Pha chế")]
        Barista = 11,

        [Description("Thu ngân")]
        Cashier = 12,

        [Description("Phục vụ")]
        Server = 13,

        [Description("Bảo vệ")]
        Security = 14
    }
}