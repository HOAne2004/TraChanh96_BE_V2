using drinking_be.Dtos.Common;
using drinking_be.Dtos.ProductDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface IProductService
    {
        // Lấy danh sách có phân trang, lọc, tìm kiếm
        Task<PagedResult<ProductReadDto>> GetAllAsync(ProductFilterDto filter);

        Task<ProductReadDto?> GetByIdAsync(int id);
        Task<ProductReadDto?> GetBySlugAsync(string slug);

        Task<ProductReadDto> CreateAsync(ProductCreateDto createDto);
        Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<StoreMenuReadDto>> GetMenuByStoreAsync(int storeId, string? search, string? categorySlug);
        Task<bool> UpdateProductStatusAtStoreAsync(ProductStoreUpdateDto updateDto);
    }
}