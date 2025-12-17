using AutoMapper;
using drinking_be.Dtos.UserDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;
using drinking_be.Enums;

namespace drinking_be.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            var userRepo = _unitOfWork.Repository<User>();
            // Lấy tất cả user, include cả Staff và Membership
            var users = await userRepo.GetAllAsync(
                includeProperties: "Staff,Membership,Membership.Level",
                orderBy: q => q.OrderByDescending(u => u.CreatedAt)
            );

            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> UpdateUserByPublicIdAsync(Guid publicId, UserUpdateDto updateDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.PublicId == publicId);

            if (user == null) return null;

            _mapper.Map(updateDto, user);

            user.UpdatedAt = DateTime.UtcNow;
            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<bool> DeleteUserByPublicIdAsync(Guid publicId)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(u => u.PublicId == publicId);

            if (user == null) return false;

            user.Status = UserStatusEnum.Deleted;
            user.DeletedAt = DateTime.UtcNow;
            
            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}