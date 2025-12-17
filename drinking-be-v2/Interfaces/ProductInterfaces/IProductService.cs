using drinking_be.Dtos.ProductDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface IProductService
    {
        // Lấy danh sách có phân trang, lọc, tìm kiếm
        Task<IEnumerable<ProductReadDto>> GetAllAsync(string? search, string? categorySlug, string? sort);

        Task<ProductReadDto?> GetByIdAsync(int id);
        Task<ProductReadDto?> GetBySlugAsync(string slug);

        Task<ProductReadDto> CreateAsync(ProductCreateDto createDto);
        Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}