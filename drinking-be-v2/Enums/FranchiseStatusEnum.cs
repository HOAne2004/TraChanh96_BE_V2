namespace drinking_be.Enums
{
    public enum FranchiseStatusEnum : short
    {
        Pending = 1,        // Mới gửi yêu cầu (Chờ xử lý)
        Contacted = 2,      // Sale đã liên hệ (Gọi điện/Email)
        Consulting = 3,     // Đang trong quá trình tư vấn/Gặp mặt/Khảo sát
        Documenting = 4,    // Đang làm hồ sơ/Thủ tục pháp lý
        Approved = 5,       // Đã duyệt (Thành công - Chuẩn bị mở Store)
        Rejected = 6,       // Từ chối (Không đủ điều kiện)
        Cancelled = 7       // Khách hàng tự hủy/Rút lui
    }
}