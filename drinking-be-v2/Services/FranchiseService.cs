using AutoMapper;
using drinking_be.Dtos.FranchiseDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class FranchiseService : IFranchiseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FranchiseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FranchiseReadDto> CreateRequestAsync(FranchiseCreateDto dto)
        {
            var repo = _unitOfWork.Repository<FranchiseRequest>();

            var request = _mapper.Map<FranchiseRequest>(dto);

            // Thiết lập mặc định
            request.Status = FranchiseStatusEnum.Pending;
            request.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FranchiseReadDto>(request);
        }

        public async Task<IEnumerable<FranchiseReadDto>> GetAllAsync(FranchiseStatusEnum? status, string? search)
        {
            var repo = _unitOfWork.Repository<FranchiseRequest>();

            // Lấy kèm thông tin Reviewer (User) để hiển thị tên người phụ trách
            var query = await repo.GetAllAsync(
                includeProperties: "Reviewer",
                orderBy: q => q.OrderByDescending(f => f.CreatedAt)
            );

            // 1. Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(f => f.Status == status.Value);
            }

            // 2. Tìm kiếm (Tên, Email, SĐT)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(f => f.FullName.ToLower().Contains(search) ||
                                         f.Email.ToLower().Contains(search) ||
                                         f.PhoneNumber.Contains(search));
            }

            return _mapper.Map<IEnumerable<FranchiseReadDto>>(query);
        }

        public async Task<FranchiseReadDto?> GetByIdAsync(int id)
        {
            var request = await _unitOfWork.Repository<FranchiseRequest>().GetFirstOrDefaultAsync(
                filter: f => f.Id == id,
                includeProperties: "Reviewer"
            );

            return request == null ? null : _mapper.Map<FranchiseReadDto>(request);
        }

        public async Task<FranchiseReadDto?> UpdateRequestAsync(int id, FranchiseUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<FranchiseRequest>();
            var request = await repo.GetByIdAsync(id);

            if (request == null) return null;

            // Map dữ liệu update (Status, Note, ReviewerId...)
            _mapper.Map(dto, request);

            request.UpdatedAt = DateTime.UtcNow;

            repo.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy tên Reviewer mới (nếu có thay đổi)
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            var repo = _unitOfWork.Repository<FranchiseRequest>();
            var request = await repo.GetByIdAsync(id);

            if (request == null) return false;

            repo.Delete(request);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}