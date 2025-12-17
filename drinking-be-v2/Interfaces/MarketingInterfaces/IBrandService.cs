// Interfaces/IBrandService.cs
using drinking_be.Dtos.BrandDtos;
using System.Threading.Tasks;

namespace drinking_be.Interfaces.MarketingInterfaces
{
    public interface IBrandService
    {
        // Lấy thông tin thương hiệu chính (Kèm Policy, SocialMedia)
        Task<BrandReadDto?> GetPrimaryBrandInfoAsync();

        // Tạo mới (Chỉ cho phép nếu chưa có)
        Task<BrandReadDto> CreateBrandAsync(BrandCreateDto brandDto);

        // Cập nhật
        Task<BrandReadDto?> UpdateBrandAsync(int id, BrandUpdateDto brandDto);
    }
}