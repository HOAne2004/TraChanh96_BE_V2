using AutoMapper;
using drinking_be.Dtos.StaffDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StaffService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StaffReadDto>> GetAllAsync(int? storeId, string? search)
        {
            var staffRepo = _unitOfWork.Repository<Staff>();

            // 1. Lấy dữ liệu kèm User và Store
            var query = await staffRepo.GetAllAsync(
                includeProperties: "User,Store"
            );

            // 2. Lọc theo Store (Nếu Manager chỉ muốn xem nhân viên quán mình)
            if (storeId.HasValue)
            {
                query = query.Where(s => s.StoreId == storeId.Value);
            }

            // 3. Tìm kiếm theo tên hoặc mã nhân viên
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.FullName.ToLower().Contains(search) ||
                                         (s.User.Email != null && s.User.Email.ToLower().Contains(search)));
            }

            return _mapper.Map<IEnumerable<StaffReadDto>>(query);
        }

        public async Task<StaffReadDto?> GetByIdAsync(int id)
        {
            var staff = await _unitOfWork.Repository<Staff>().GetFirstOrDefaultAsync(
                filter: s => s.Id == id,
                includeProperties: "User,Store"
            );

            return staff == null ? null : _mapper.Map<StaffReadDto>(staff);
        }

        public async Task<StaffReadDto?> GetByUserIdAsync(int userId)
        {
            var staff = await _unitOfWork.Repository<Staff>().GetFirstOrDefaultAsync(
                filter: s => s.UserId == userId,
                includeProperties: "User,Store"
            );

            return staff == null ? null : _mapper.Map<StaffReadDto>(staff);
        }

        public async Task<StaffReadDto> CreateAsync(StaffCreateDto createDto)
        {
            var staffRepo = _unitOfWork.Repository<Staff>();

            // Validate: 1 User chỉ có 1 hồ sơ Staff
            var existingStaff = await staffRepo.GetFirstOrDefaultAsync(s => s.UserId == createDto.UserId);
            if (existingStaff != null)
            {
                throw new Exception("User này đã có hồ sơ nhân viên rồi.");
            }

            var staff = _mapper.Map<Staff>(createDto);

            // PublicId tự sinh trong DB hoặc gán tại đây
            staff.PublicId = Guid.NewGuid();

            await staffRepo.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy thông tin User/Store hiển thị ra
            return (await GetByIdAsync(staff.Id))!;
        }

        public async Task<StaffReadDto?> UpdateAsync(int id, StaffUpdateDto updateDto)
        {
            var staffRepo = _unitOfWork.Repository<Staff>();
            var staff = await staffRepo.GetByIdAsync(id);

            if (staff == null) return null;

            _mapper.Map(updateDto, staff);

            staff.UpdatedAt = DateTime.UtcNow;

            staffRepo.Update(staff);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staffRepo = _unitOfWork.Repository<Staff>();
            var staff = await staffRepo.GetByIdAsync(id);

            if (staff == null) return false;

            staffRepo.Delete(staff);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}