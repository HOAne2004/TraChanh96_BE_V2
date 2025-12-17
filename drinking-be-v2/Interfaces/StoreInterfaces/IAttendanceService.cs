using drinking_be.Dtos.AttendanceDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IAttendanceService
    {
        // --- Dành cho Nhân viên ---
        Task<AttendanceReadDto> CheckInAsync(int staffId);
        Task<AttendanceReadDto> CheckOutAsync(int staffId);
        Task<AttendanceReadDto?> GetTodayAttendanceAsync(int staffId); // Xem hôm nay đã chấm chưa

        // --- Dành cho Manager ---
        // Xem công của 1 nhân viên trong tháng
        Task<IEnumerable<AttendanceReadDto>> GetStaffHistoryAsync(int staffId, int month, int year);

        // Xem công của cả cửa hàng trong ngày (để điểm danh)
        Task<IEnumerable<AttendanceReadDto>> GetStoreDailyReportAsync(int storeId, DateOnly date);

        // Quản lý sửa công/thưởng phạt
        Task<AttendanceReadDto?> UpdateAsync(long id, AttendanceUpdateDto updateDto);

        // Tạo công thủ công (nếu cần)
        Task<AttendanceReadDto> CreateManualAsync(AttendanceCreateDto createDto);
    }
}