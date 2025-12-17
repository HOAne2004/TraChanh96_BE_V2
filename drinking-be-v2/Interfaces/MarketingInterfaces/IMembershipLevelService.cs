using drinking_be.Dtos.MembershipLevelDtos;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IMembershipLevelService
    {
        // Lấy danh sách cấp độ (Sắp xếp theo số tiền chi tiêu tăng dần)
        Task<IEnumerable<MembershipLevelReadDto>> GetAllLevelsAsync();

        // Lấy chi tiết
        Task<MembershipLevelReadDto?> GetByIdAsync(byte id);

        // Tạo mới
        Task<MembershipLevelReadDto> CreateLevelAsync(MembershipLevelCreateDto dto);

        // Cập nhật
        Task<MembershipLevelReadDto?> UpdateLevelAsync(byte id, MembershipLevelUpdateDto dto);

        // Xóa mềm
        Task<bool> DeleteLevelAsync(byte id);
    }
}