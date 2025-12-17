using AutoMapper;
using drinking_be.Dtos.MembershipDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // ✅ Constructor mới: Chỉ nhận UnitOfWork và Mapper
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MembershipReadDto?> GetMyMembershipAsync(int userId)
        {
            var repo = _unitOfWork.Repository<Membership>();

            // Lấy membership của user, kèm thông tin Hạng và User
            var membership = await repo.GetFirstOrDefaultAsync(
                filter: m => m.UserId == userId,
                includeProperties: "Level,User"
            );

            return membership == null ? null : _mapper.Map<MembershipReadDto>(membership);
        }

        public async Task<MembershipReadDto?> GetByIdAsync(long id)
        {
            var membership = await _unitOfWork.Repository<Membership>().GetFirstOrDefaultAsync(
                filter: m => m.Id == id,
                includeProperties: "Level,User"
            );
            return membership == null ? null : _mapper.Map<MembershipReadDto>(membership);
        }

        public async Task<MembershipReadDto> CreateMembershipAsync(MembershipCreateDto dto)
        {
            var repo = _unitOfWork.Repository<Membership>();

            // 1. Kiểm tra User đã có thẻ chưa
            var existing = await repo.GetFirstOrDefaultAsync(m => m.UserId == dto.UserId);
            if (existing != null)
            {
                throw new Exception("Người dùng này đã có thẻ thành viên.");
            }

            // 2. Map và xử lý dữ liệu
            var membership = _mapper.Map<Membership>(dto);

            // Nếu không truyền mã thẻ, tự sinh ngẫu nhiên
            if (string.IsNullOrEmpty(membership.CardCode))
            {
                membership.CardCode = $"MEM-{dto.UserId}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";
            }

            membership.CreatedAt = DateTime.UtcNow;
            membership.Status = MembershipStatusEnum.Active;

            // 3. Lưu vào DB
            await repo.AddAsync(membership);
            await _unitOfWork.SaveChangesAsync();

            // 4. Load lại để lấy thông tin Level/User hiển thị
            return (await GetByIdAsync(membership.Id))!;
        }
    }
}