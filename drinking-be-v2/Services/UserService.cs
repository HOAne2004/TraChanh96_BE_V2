using AutoMapper;
using drinking_be.Dtos.Common;

using drinking_be.Dtos.UserDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserReadDto>> GetAllAsync(UserFilterDto filter){
            var query = _unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.Membership).ThenInclude(m => m.Level)
                .AsNoTracking();

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == filter.RoleId);
            }
            if (filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == filter.Status);
            }
            if (filter.MembershipLevelId.HasValue)
            {
                query = query.Where(u => u.Membership != null && u.Membership.MembershipLevelId == filter.MembershipLevelId);
            }
            if (filter.FromDate.HasValue)
            {
                var from = DateTime.SpecifyKind(filter.FromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(u => u.CreatedAt >= from);
            }
            if (filter.ToDate.HasValue)
            {
                var to = DateTime.SpecifyKind(filter.ToDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(u => u.CreatedAt <= to);
            }

            int totalRow = await query.CountAsync();
            query = query.OrderByDescending(u => u.CreatedAt);
            var items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            var resultDtos = _mapper.Map<List<UserReadDto>>(items);
            return new PagedResult<UserReadDto>(resultDtos, totalRow, filter.PageIndex, filter.PageSize);
        }
        public async Task<UserReadDto?> GetUserByPublicIdAsync(Guid publicId)
        {
            var userRepo = _unitOfWork.Repository<User>();
            // Include Membership để hiển thị hạng thành viên trong Profile
            var user = await userRepo.GetFirstOrDefaultAsync(
                filter: u => u.PublicId == publicId,
                includeProperties: "Membership,Membership.Level"
            );

            return _mapper.Map<UserReadDto>(user);
        }
        public async Task<UserReadDto?> UpdateUserByPublicIdAsync(Guid publicId, UserUpdateDto updateDto)
        {
            var userRepo = _unitOfWork.Repository<User>();

            var user = await userRepo.GetFirstOrDefaultAsync(u => u.PublicId == publicId);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(updateDto.Email) &&
                !string.Equals(updateDto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var isDuplicate = await _unitOfWork.Repository<User>().GetQueryable()
                    .AnyAsync(u => u.Email == updateDto.Email && u.Id != user.Id);

                if (isDuplicate)
                {
                    throw new Exception($"Email '{updateDto.Email}' đã được sử dụng bởi tài khoản khác.");
                }
            }

            _mapper.Map(updateDto, user);

            user.UpdatedAt = DateTime.UtcNow;

            userRepo.Update(user);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_user_email"))
                {
                    throw new Exception("Lỗi hệ thống: Email này đang bị trùng lặp trong cơ sở dữ liệu (có thể do tài khoản đã xóa).");
                }
                throw;
            }

            return _mapper.Map<UserReadDto>(user);
        }
        public async Task<bool> DeleteUserByPublicIdAsync(Guid publicId)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.PublicId == publicId);

            if (user == null) return false;

            user.DeletedAt = DateTime.UtcNow;
            user.Status = Enums.UserStatusEnum.Deleted;
            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}