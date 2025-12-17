using AutoMapper;
using drinking_be.Dtos.MembershipLevelDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class MembershipLevelService : IMembershipLevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipLevelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MembershipLevelReadDto>> GetAllLevelsAsync()
        {
            var repo = _unitOfWork.Repository<MembershipLevel>();

            // Lấy danh sách, sắp xếp theo mức chi tiêu yêu cầu (MinSpendRequired)
            // Lọc Status Active nếu cần (nhưng Admin thì nên thấy hết)
            var levels = await repo.GetAllAsync(
                filter: l => l.Status != PublicStatusEnum.Inactive, // Ví dụ lọc cái đã xóa
                orderBy: q => q.OrderBy(l => l.MinSpendRequired)
            );

            return _mapper.Map<IEnumerable<MembershipLevelReadDto>>(levels);
        }

        public async Task<MembershipLevelReadDto?> GetByIdAsync(byte id)
        {
            var repo = _unitOfWork.Repository<MembershipLevel>();

            // Include Memberships và VoucherTemplates để đếm số lượng (cho Admin dashboard)
            var level = await repo.GetFirstOrDefaultAsync(
                filter: l => l.Id == id,
                includeProperties: "Memberships,VoucherTemplates"
            );

            return level == null ? null : _mapper.Map<MembershipLevelReadDto>(level);
        }

        public async Task<MembershipLevelReadDto> CreateLevelAsync(MembershipLevelCreateDto dto)
        {
            var repo = _unitOfWork.Repository<MembershipLevel>();

            // 1. Kiểm tra tên trùng lặp
            var existing = await repo.GetFirstOrDefaultAsync(l => l.Name.ToLower() == dto.Name.ToLower());
            if (existing != null)
            {
                throw new Exception("Tên cấp độ này đã tồn tại.");
            }

            // 2. Map và Tạo mới
            var level = _mapper.Map<MembershipLevel>(dto);
            level.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(level);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipLevelReadDto>(level);
        }

        public async Task<MembershipLevelReadDto?> UpdateLevelAsync(byte id, MembershipLevelUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<MembershipLevel>();
            var level = await repo.GetByIdAsync(id);

            if (level == null) return null;

            // Map dữ liệu update
            _mapper.Map(dto, level);
            level.UpdatedAt = DateTime.UtcNow;

            repo.Update(level);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MembershipLevelReadDto>(level);
        }

        public async Task<bool> DeleteLevelAsync(byte id)
        {
            var repo = _unitOfWork.Repository<MembershipLevel>();
            var level = await repo.GetByIdAsync(id);

            if (level == null) return false;

            // Soft Delete
            level.Status = PublicStatusEnum.Inactive;
            level.DeletedAt = DateTime.UtcNow;

            repo.Update(level);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}