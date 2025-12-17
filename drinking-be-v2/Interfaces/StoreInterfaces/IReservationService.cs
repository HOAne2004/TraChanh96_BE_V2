using drinking_be.Dtos.ReservationDtos;
using drinking_be.Enums;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IReservationService
    {
        // Tạo đơn đặt bàn
        Task<ReservationReadDto> CreateReservationAsync(ReservationCreateDto dto);

        // Lấy chi tiết
        Task<ReservationReadDto?> GetReservationByIdAsync(long id);

        // Lấy lịch sử của User
        Task<IEnumerable<ReservationReadDto>> GetHistoryByUserIdAsync(int userId);

        // Admin/Manager: Xem danh sách theo Store và Ngày
        Task<IEnumerable<ReservationReadDto>> GetReservationsByStoreAsync(int storeId, DateTime? date, ReservationStatusEnum? status);

        // Admin: Cập nhật (Gán bàn, đổi trạng thái)
        Task<ReservationReadDto?> UpdateReservationAsync(long id, ReservationUpdateDto dto);

        // Hủy đơn (Có logic kiểm tra thời gian và cọc)
        Task<bool> CancelReservationAsync(long id, int? userId, string reason);
    }
}