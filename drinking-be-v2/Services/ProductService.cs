using AutoMapper;
using drinking_be.Dtos.ProductDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Để dùng SlugGenerator
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllAsync(string? search, string? categorySlug, string? sort)
        {
            var productRepo = _unitOfWork.Repository<Product>();

            // 1. Lấy dữ liệu kèm Category và Size (Eager Loading)
            // Chuỗi include phải khớp tên Property trong Entity Product
            var products = await productRepo.GetAllAsync(
                includeProperties: "Category,ProductSizes,ProductSizes.Size"
            );

            // 2. Lọc theo tên (Search)
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            // 3. Lọc theo Category Slug
            if (!string.IsNullOrEmpty(categorySlug))
            {
                products = products.Where(p => p.Category != null && p.Category.Slug == categorySlug);
            }

            // 4. Sắp xếp
            if (!string.IsNullOrEmpty(sort))
            {
                products = sort switch
                {
                    "price_asc" => products.OrderBy(p => p.BasePrice),
                    "price_desc" => products.OrderByDescending(p => p.BasePrice),
                    "newest" => products.OrderByDescending(p => p.CreatedAt),
                    _ => products
                };
            }

            return _mapper.Map<IEnumerable<ProductReadDto>>(products);
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                filter: p => p.Id == id,
                includeProperties: "Category,ProductSizes,ProductSizes.Size"
            );

            return product == null ? null : _mapper.Map<ProductReadDto>(product);
        }

        public async Task<ProductReadDto?> GetBySlugAsync(string slug)
        {
            var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                filter: p => p.Slug == slug,
                includeProperties: "Category,ProductSizes,ProductSizes.Size"
            );

            return product == null ? null : _mapper.Map<ProductReadDto>(product);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto createDto)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var productSizeRepo = _unitOfWork.Repository<ProductSize>();

            // 1. Map DTO -> Entity
            var product = _mapper.Map<Product>(createDto);

            // 2. Tạo Slug và PublicId
            product.Slug = SlugGenerator.GenerateSlug(product.Name);
            product.PublicId = Guid.NewGuid().ToString();

            // 3. Lưu Product trước để lấy ID
            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // 4. Xử lý thêm Size (Nếu có chọn Size)
            if (createDto.SizeIds != null && createDto.SizeIds.Any())
            {
                foreach (var sizeId in createDto.SizeIds)
                {
                    var productSize = new ProductSize
                    {
                        ProductId = product.Id,
                        SizeId = (short)sizeId // Ép kiểu về short vì SizeId là short
                    };
                    await productSizeRepo.AddAsync(productSize);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            // 5. Load lại để trả về full thông tin
            return (await GetByIdAsync(product.Id))!;
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto updateDto)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var productSizeRepo = _unitOfWork.Repository<ProductSize>();

            var product = await productRepo.GetFirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            // 1. Map thông tin cơ bản
            _mapper.Map(updateDto, product);

            // Nếu đổi tên thì đổi Slug (tùy logic, thường hạn chế đổi slug để SEO)
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                product.Slug = SlugGenerator.GenerateSlug(updateDto.Name);
            }

            product.UpdatedAt = DateTime.UtcNow;
            productRepo.Update(product);

            // 2. Xử lý cập nhật Size (Xóa cũ -> Thêm mới) - Logic đơn giản nhất
            if (updateDto.SizeIds != null)
            {
                // Lấy các size cũ của product này
                var oldSizes = await productSizeRepo.GetAllAsync(filter: ps => ps.ProductId == id);

                // Xóa hết size cũ
                productSizeRepo.DeleteRange(oldSizes);

                // Thêm size mới
                foreach (var sizeId in updateDto.SizeIds)
                {
                    await productSizeRepo.AddAsync(new ProductSize
                    {
                        ProductId = id,
                        SizeId = (short)sizeId
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var product = await productRepo.GetByIdAsync(id);

            if (product == null) return false;

            // Soft Delete (Xóa mềm)
            // product.DeletedAt = DateTime.UtcNow;
            // productRepo.Update(product);

            // Hard Delete (Xóa cứng)
            productRepo.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}