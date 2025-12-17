// File: Enums/ContentStatusEnum.cs
using System.ComponentModel;

namespace drinking_be.Enums
{
    public enum ContentStatusEnum : byte
    {
        // Nhóm 1: Trạng thái Khởi tạo & Kiểm duyệt

        [Description("Bản nháp")]
        Draft = 1, // Bài viết đang được tạo, chưa hoàn thiện

        [Description("Chờ kiểm duyệt")]
        PendingReview = 2, // Đã gửi lên, chờ Admin/Editor xem xét

        // Nhóm 2: Trạng thái Kết quả Kiểm duyệt & Công khai

        [Description("Đã xuất bản")]
        Published = 3, // Tương ứng với Active. Bài viết hiển thị công khai (Active)

        [Description("Đã từ chối")]
        Rejected = 4, // Bài viết không được duyệt, cần chỉnh sửa lại

        [Description("Đã gỡ xuống")]
        Unpublished = 5, // Bài viết đã từng Published nhưng bị gỡ (Tương ứng với Inactive)

        // Nhóm 3: Trạng thái Xóa mềm (Soft Delete)

        [Description("Đã bị xóa")]
        Deleted = 99 // Đưa vào thùng rác
    }
}