using drinking_be.Dtos.MembershipDtos;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IMembershipService
    {
        // Lấy thông tin thành viên của user hiện tại
        Task<MembershipReadDto?> GetMyMembershipAsync(int userId);

        // Tạo mới (Thường dùng nội bộ khi đăng ký user hoặc Admin tạo thủ công)
        Task<MembershipReadDto> CreateMembershipAsync(MembershipCreateDto dto);

        // Admin: Lấy chi tiết theo ID
        Task<MembershipReadDto?> GetByIdAsync(long id);
    }
}