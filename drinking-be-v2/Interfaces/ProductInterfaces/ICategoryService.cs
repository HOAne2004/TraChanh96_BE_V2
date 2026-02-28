// Interfaces/CategoryInterfaces/ICategoryService.cs
using drinking_be.Dtos.CategoryDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync();
        Task<CategoryReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<CategoryReadDto>> GetActiveCateAsync(); 
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto createDto);
        Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}