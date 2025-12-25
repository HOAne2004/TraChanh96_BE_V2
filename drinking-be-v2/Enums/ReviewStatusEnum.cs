// File: Enums/ReviewStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum ReviewStatusEnum : short
    {
        // Nhóm 1: Kiểm duyệt

        [Description("Chờ xét duyệt")]
        Pending = 1, // Nội dung vừa được gửi, chờ Admin kiểm tra

        [Description("Đã duyệt")]
        Approved = 2, // Nội dung hiển thị công khai (Tương ứng với Active)

        [Description("Đã từ chối")]
        Rejected = 3, // Nội dung không hợp lệ, không hiển thị

        // Nhóm 2: Xóa mềm

        [Description("Đã bị xóa")]
        Deleted = 99 // Nội dung bị gỡ xuống/xóa mềm
    }
}