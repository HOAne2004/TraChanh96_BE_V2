using AutoMapper;
using drinking_be.Dtos.StaffDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static drinking_be.Services.OrderService;

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
            var userRepo = _unitOfWork.Repository<User>();

            // 1. KONTROL SỐ LƯỢNG VÀ TÍNH HỢP LỆ CỦA CƠ SỞ
            bool isStoreRole = createDto.Position >= StaffPositionEnum.StoreManager; // Từ 10 trở lên là role cửa hàng

            if (isStoreRole && !createDto.StoreId.HasValue)
            {
                throw new AppException("Nhân viên thuộc cửa hàng bắt buộc phải chọn Cơ sở làm việc.");
            }

            if (!isStoreRole && createDto.StoreId.HasValue)
            {
                throw new AppException("Nhân viên Văn phòng (HQ) không được gắn vào một cơ sở cụ thể.");
            }

            if (createDto.StoreId.HasValue && createDto.Position == StaffPositionEnum.StoreManager)
            {
                var managerCount = await staffRepo.GetQueryable()
                    .CountAsync(s => s.StoreId == createDto.StoreId.Value &&
                                     s.Position == StaffPositionEnum.StoreManager &&
                                     s.Status != PublicStatusEnum.Inactive && s.Status != PublicStatusEnum.Deleted);

                if (managerCount >= 2) throw new Exception("Cửa hàng này đã đạt giới hạn tối đa 2 Quản lý.");
            }

            // 2. 🟢 LOGIC USER: Tìm hoặc tạo mới User
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.Email.ToLower() == createDto.Email.ToLower());

            if (user == null)
            {
                // Tạo mới tài khoản cho nhân viên
                string defaultPassword = string.IsNullOrEmpty(createDto.Password) ? "Staff@123" : createDto.Password;
                user = new User
                {
                    Email = createDto.Email,
                    Username = createDto.Email.Split('@')[0], // Cắt phần đầu email làm username
                    Phone = createDto.Phone,
                    // Lưu ý: Nếu bạn dùng thư viện khác để Hash, hãy đổi lại dòng này
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                    RoleId = createDto.Position == StaffPositionEnum.StoreManager ? UserRoleEnum.Manager : UserRoleEnum.Staff,
                    Status = UserStatusEnum.Active,
                    EmailVerified = false,
                    PublicId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await userRepo.AddAsync(user);
                await _unitOfWork.SaveChangesAsync(); // Save để lấy user.Id
            }
            else
            {
                // User đã tồn tại -> Kiểm tra xem đã có hồ sơ nhân viên chưa
                var existingStaff = await staffRepo.GetFirstOrDefaultAsync(s => s.UserId == user.Id && s.Status != PublicStatusEnum.Deleted);
                if (existingStaff != null) throw new Exception("Tài khoản Email này đã có hồ sơ nhân viên.");

                // Đồng bộ cập nhật lại quyền
                user.RoleId = createDto.Position == StaffPositionEnum.StoreManager ? UserRoleEnum.Manager : UserRoleEnum.Staff;
                userRepo.Update(user);
            }

            // 3. TẠO STAFF
            var staff = _mapper.Map<Staff>(createDto);
            staff.UserId = user.Id; // Gắn ID User vào
            staff.PublicId = Guid.NewGuid();
            staff.Status = PublicStatusEnum.Active;

            await staffRepo.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(staff.Id))!;
        }

        public async Task<StaffReadDto?> UpdateAsync(int id, StaffUpdateDto updateDto)
        {
            bool isStoreRole = updateDto.Position >= StaffPositionEnum.StoreManager; // Từ 10 trở lên là role cửa hàng

            if (isStoreRole && !updateDto.StoreId.HasValue)
            {
                throw new AppException("Nhân viên thuộc cửa hàng bắt buộc phải chọn Cơ sở làm việc.");
            }

            if (!isStoreRole && updateDto.StoreId.HasValue)
            {
                throw new AppException("Nhân viên Văn phòng (HQ) không được gắn vào một cơ sở cụ thể.");
            }

            var staffRepo = _unitOfWork.Repository<Staff>();
            var staff = await staffRepo.GetByIdAsync(id);
            if (staff == null) return null;

            _mapper.Map(updateDto, staff);
            staff.UpdatedAt = DateTime.UtcNow;
            staffRepo.Update(staff);

            if (updateDto.Position.HasValue)
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(staff.UserId);
                if (user != null && user.RoleId != UserRoleEnum.Admin)
                {
                    user.RoleId = updateDto.Position == StaffPositionEnum.StoreManager ? UserRoleEnum.Manager : UserRoleEnum.Staff;
                    _unitOfWork.Repository<User>().Update(user);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staffRepo = _unitOfWork.Repository<Staff>();
            var staff = await staffRepo.GetByIdAsync(id);
            if (staff == null) return false;

            staff.Status = PublicStatusEnum.Deleted;
            staff.StoreId = null;
            staff.UpdatedAt = DateTime.UtcNow;
            staffRepo.Update(staff);

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(staff.UserId);
            if (user != null)
            {
                user.RoleId = UserRoleEnum.Customer; // Hạ bệ thành khách hàng bình thường
                _unitOfWork.Repository<User>().Update(user);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}