using AutoMapper;
using drinking_be.Dtos.UserDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Models;

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

            userRepo.Delete(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}