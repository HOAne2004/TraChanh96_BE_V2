using AutoMapper;
using drinking_be.Dtos.CategoryDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using System.Security.AccessControl;

namespace drinking_be.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
        {
            // Sử dụng Generic Repository thông qua UnitOfWork
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync(
                orderBy: q => q.OrderBy(c => c.SortOrder).ThenBy(c => c.Id));
            return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null) return null;
            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<IEnumerable<CategoryReadDto>> GetActiveCateAsync()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync(
                filter: c => c.Status == PublicStatusEnum.Active,
                orderBy: q => q.OrderBy(c => c.SortOrder).ThenBy(c => c.Name));
            return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        }
        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto createDto)
        {
            var category = _mapper.Map<Category>(createDto);

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto updateDto)
        {
            var repo = _unitOfWork.Repository<Category>();
            var category = await repo.GetByIdAsync(id);

            if (category == null) return null;

            _mapper.Map(updateDto, category);

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<Category>();
            var category = await repo.GetByIdAsync(id);

            if (category == null) return false;

            category.Status = PublicStatusEnum.Deleted;
            category.DeletedAt = DateTime.UtcNow;

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}