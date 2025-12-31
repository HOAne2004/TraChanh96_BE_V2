using AutoMapper;
using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentMethodService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentMethodReadDto>> GetActiveMethodsAsync()
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();

            // Lấy Active và sắp xếp theo SortOrder (nhỏ lên trước)
            var methods = await repo.GetAllAsync(
                filter: p => p.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
            );

            return _mapper.Map<IEnumerable<PaymentMethodReadDto>>(methods);
        }

        public async Task<IEnumerable<PaymentMethodReadDto>> GetAllMethodsAsync()
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();

            // Admin thấy hết (cả Active, Inactive, Hidden)
            var methods = await repo.GetAllAsync(
                orderBy: q => q.OrderBy(p => p.SortOrder)
            );

            return _mapper.Map<IEnumerable<PaymentMethodReadDto>>(methods);
        }

        public async Task<PaymentMethodReadDto?> GetByIdAsync(int id)
        {
            var method = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(id);
            return method == null ? null : _mapper.Map<PaymentMethodReadDto>(method);
        }

        public async Task<PaymentMethodReadDto> CreateAsync(PaymentMethodCreateDto dto)
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();

            var method = _mapper.Map<PaymentMethod>(dto);
            method.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(method);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentMethodReadDto>(method);
        }

        public async Task<PaymentMethodReadDto?> UpdateAsync(int id, PaymentMethodUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();
            var method = await repo.GetByIdAsync(id);

            if (method == null) return null;

            _mapper.Map(dto, method);
            method.UpdatedAt = DateTime.UtcNow;

            repo.Update(method);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentMethodReadDto>(method);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();
            var method = await repo.GetByIdAsync(id);

            if (method == null) return false;

            // Soft Delete
            method.Status = PublicStatusEnum.Inactive;
            method.DeletedAt = DateTime.UtcNow;

            repo.Update(method);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var repo = _unitOfWork.Repository<PaymentMethod>();
            var method = await repo.GetByIdAsync(id);

            if (method == null) return false;

            // Đảo trạng thái: Active <-> Inactive
            method.Status = method.Status == PublicStatusEnum.Active
                ? PublicStatusEnum.Inactive
                : PublicStatusEnum.Active;

            repo.Update(method);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}