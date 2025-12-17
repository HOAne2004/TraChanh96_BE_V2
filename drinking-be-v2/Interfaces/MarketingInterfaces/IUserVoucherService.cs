using drinking_be.Dtos.VoucherDtos;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IUserVoucherService
    {
        // User: Lấy danh sách voucher của tôi (Còn hạn, chưa dùng)
        Task<IEnumerable<UserVoucherReadDto>> GetMyVouchersAsync(int userId);

        // User: Kiểm tra và tính toán giảm giá (Pre-check trước khi đặt hàng)
        Task<VoucherApplyResultDto> ApplyVoucherAsync(int userId, VoucherApplyDto applyDto);

        // Admin: Tặng voucher cho User
        Task<UserVoucherReadDto> IssueVoucherAsync(UserVoucherCreateDto dto);

        // System: Đánh dấu voucher đã dùng (Gọi khi tạo đơn hàng thành công)
        Task MarkVoucherUsedAsync(long userVoucherId, long orderId);

        // System: Hoàn lại voucher (Gọi khi hủy đơn hàng)
        Task RestoreVoucherAsync(long userVoucherId);
    }
}