using AutoMapper;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.ProductDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Để dùng SlugGenerator
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task<PagedResult<ProductReadDto>> GetAllAsync(ProductFilterDto filter)
        {
            // 1. Khởi tạo Query (IQueryable) - Chưa chạy xuống DB ngay
            // Lưu ý: Dùng AsNoTracking() để tăng tốc độ cho thao tác chỉ đọc
            var query = _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductStores) // Include để lấy StoreIds sau này
                .AsNoTracking();

            // 2. --- ÁP DỤNG BỘ LỌC (FILTERING) ---

            // Lọc theo Trạng thái (Active, Inactive, Deleted...)
            if (filter.Status.HasValue)
            {
                query = query.Where(p => p.Status == filter.Status);
            }
            else
            {
                // Mặc định ẩn sản phẩm đã xóa nếu không chọn status cụ thể (Soft Delete logic)
                // Nếu muốn admin xem được cả hàng đã xóa thì bỏ dòng này hoặc thêm logic riêng
                query = query.Where(p => p.Status != ProductStatusEnum.Deleted);
            }

            // Lọc theo Loại sản phẩm (Drink, Food...)
            if (filter.Type.HasValue)
            {
                query = query.Where(p => p.ProductType == filter.Type);
            }

            // Lọc theo Danh mục
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId);
            }

            // Lọc theo Từ khóa (Tên, Slug) - Case insensitive
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var k = filter.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(k) ||
                    p.Slug.ToLower().Contains(k)
                );
            }

            // Lọc theo Ngày tạo (FromDate - ToDate)
            if (filter.FromDate.HasValue)
            {
                // Chuyển về UTC nếu DB lưu UTC
                var from = DateTime.SpecifyKind(filter.FromDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(p => p.CreatedAt >= from);
            }
            if (filter.ToDate.HasValue)
            {
                // Lấy đến hết ngày (23:59:59)
                var to = DateTime.SpecifyKind(filter.ToDate.Value.Date.AddDays(1), DateTimeKind.Utc);
                query = query.Where(p => p.CreatedAt < to);
            }

            // 3. --- PAGING & SORTING ---

            // Tính tổng số bản ghi trước khi phân trang
            int totalRow = await query.CountAsync();

            // Sắp xếp (Mặc định mới nhất lên đầu)
            // Nếu filter có trường Sort thì switch case ở đây, hiện tại để mặc định:
            query = query.OrderByDescending(p => p.CreatedAt);

            // Lấy dữ liệu phân trang (Skip/Take)
            var items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // 4. --- MAPPING & XỬ LÝ STORE IDS ---

            var productDtos = _mapper.Map<List<ProductReadDto>>(items);

            // Gán StoreIds thủ công (Logic cũ của bạn, nhưng giờ chỉ chạy trên 10-20 item của trang hiện tại -> Rất nhanh)
            // Vì danh sách 'items' và 'productDtos' có cùng thứ tự index do Map list-to-list
            for (int i = 0; i < productDtos.Count; i++)
            {
                var entity = items[i];
                if (entity.ProductStores != null && entity.ProductStores.Any())
                {
                    productDtos[i].StoreIds = entity.ProductStores.Select(ps => ps.StoreId).ToList();
                }
            }

            // 5. Trả về kết quả phân trang
            return new PagedResult<ProductReadDto>(productDtos, totalRow, filter.PageIndex, filter.PageSize);
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
            var productStoreRepo = _unitOfWork.Repository<ProductStore>(); // 🆕 Repo mới
            var storeRepo = _unitOfWork.Repository<Store>(); // 🆕 Để lấy danh sách cửa hàng

            // 1. Map DTO -> Entity (Lúc này AutoMapper đã map luôn ProductSizes nhờ cấu hình ở trên)
            var product = _mapper.Map<Product>(createDto);
            product.BrandId = 1;
            product.Slug = SlugGenerator.GenerateSlug(product.Name);
            product.PublicId = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            // 2. Lưu Product (Kèm ProductSizes nhờ EF Core thông minh tự hiểu nested list)
            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(); // Save lần 1 để có ProductId

            // 3. ⭐ LOGIC MỚI: Tự động phân phối sản phẩm về các Cửa hàng (Stores)
            // Lấy tất cả cửa hàng đang hoạt động
            var activeStores = await storeRepo.GetAllAsync(s => s.Status == Enums.StoreStatusEnum.Active); // Giả sử bạn có Enum Active

            if (activeStores.Any())
            {
                var productStores = new List<ProductStore>();
                foreach (var store in activeStores)
                {
                    productStores.Add(new ProductStore
                    {
                        ProductId = product.Id,
                        StoreId = store.Id,
                        Status = Enums.ProductStoreStatusEnum.Available, // Mặc định là có bán
                        PriceOverride = null, // Mặc định theo giá chung
                        SoldCount = 0
                    });
                }
                await productStoreRepo.AddRangeAsync(productStores);
                await _unitOfWork.SaveChangesAsync(); // Save lần 2
            }

            // 4. Load lại và trả về
            return (await GetByIdAsync(product.Id))!;
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto updateDto)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var productSizeRepo = _unitOfWork.Repository<ProductSize>();

            // Include ProductSizes để tý nữa còn xóa
            var product = await productRepo.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "ProductSizes");
            if (product == null) return null;

            // 1. Map thông tin cơ bản
            _mapper.Map(updateDto, product);

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                // Kiểm tra xem tên có đổi thật không thì mới đổi slug (để tránh SEO bị hỏng)
                if (product.Name != updateDto.Name)
                    product.Slug = SlugGenerator.GenerateSlug(updateDto.Name);
            }

            product.UpdatedAt = DateTime.UtcNow;
            productRepo.Update(product);

            // 2. ⭐ Xử lý cập nhật Size (Phiên bản mới)
            if (updateDto.ProductSizes != null)
            {
                // Cách an toàn nhất cho Many-to-Many: Xóa hết cũ -> Thêm mới
                // (Trừ khi bạn muốn giữ lịch sử hoặc ID, nhưng ProductSize là bảng phụ thuộc nên xóa ok)

                // Lấy danh sách size cũ từ DB (dựa vào include ở trên hoặc query lại)
                if (product.ProductSizes != null && product.ProductSizes.Any())
                {
                    productSizeRepo.DeleteRange(product.ProductSizes);
                }

                // Thêm danh sách mới từ DTO
                foreach (var sizeDto in updateDto.ProductSizes)
                {
                    await productSizeRepo.AddAsync(new ProductSize
                    {
                        ProductId = id,
                        SizeId = sizeDto.SizeId,
                        PriceOverride = sizeDto.PriceOverride, // ✅ Lấy giá override mới
                        Status = Enums.PublicStatusEnum.Active
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
            product.DeletedAt = DateTime.UtcNow;
            product.Status = ProductStatusEnum.Deleted;
            productRepo.Update(product);

            // Hard Delete (Xóa cứng)
            //productRepo.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            // Lưu ý: GetByIdAsync thường mặc định lọc bỏ DeletedAt != null
            // Nên bạn có thể cần dùng GetFirstOrDefaultAsync với IgnoreQueryFilters nếu repo hỗ trợ, 
            // hoặc query trực tiếp DB set.
            // Ở đây giả sử Repository của bạn cho phép lấy theo ID dù đã xóa mềm (hoặc bạn viết hàm GetDeletedByIdAsync riêng)

            var product = await productRepo.GetQueryable()
                .IgnoreQueryFilters() // Quan trọng: Bỏ qua filter mặc định để tìm thấy món đã xóa
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return false;

            product.DeletedAt = null; // Xóa dấu vết ngày xóa
            product.Status = Enums.ProductStatusEnum.Inactive; // Khôi phục về Inactive (để Admin duyệt lại) hoặc Active tùy bạn

            productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<StoreMenuReadDto>> GetMenuByStoreAsync(int storeId, string? search, string? categorySlug)
        {
            var productRepo = _unitOfWork.Repository<Product>();

            // 1. Lấy danh sách ProductStore của cửa hàng này
            // Ta cần join bảng ProductStore với Product để lấy thông tin món
            // Query: Lấy Product, Include ProductStore (lọc theo storeId)
            // Tuy nhiên, Repository Pattern Generic thường khó join custom. 
            // Cách đơn giản và hiệu quả với EF Core: Lấy từ Product, Include ProductStores

            var query = productRepo.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductStores) // Include để check trạng thái tại store
                .Where(p => p.Status == Enums.ProductStatusEnum.Active); // Chỉ lấy món TCT đang bán

            // 2. Filter Search & Category (Giống GetAll)
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            if (!string.IsNullOrEmpty(categorySlug))
                query = query.Where(p => p.Category.Slug == categorySlug);

            var products = await query.ToListAsync();

            // 3. Map sang DTO và xử lý logic Store
            var result = new List<StoreMenuReadDto>();

            foreach (var p in products)
            {
                // Tìm thông tin món này tại cửa hàng
                var storeData = p.ProductStores.FirstOrDefault(ps => ps.StoreId == storeId);

                // Nếu không tìm thấy storeData (lỗi data) hoặc StoreStatus == Disabled/Hidden -> Bỏ qua không hiện
                if (storeData == null ||
                    storeData.Status == Enums.ProductStoreStatusEnum.Disabled ||
                    storeData.Status == Enums.ProductStoreStatusEnum.Hidden)
                {
                    continue;
                }

                // Map cơ bản
                var dto = _mapper.Map<StoreMenuReadDto>(p);

                // --- LOGIC CỬA HÀNG ---
                dto.StoreStatus = storeData.Status.ToString();

                // Logic IsSoldOut: Nếu trạng thái là OutOfStock -> True
                dto.IsSoldOut = (storeData.Status == Enums.ProductStoreStatusEnum.OutOfStock);

                // Logic DisplayPrice: Nếu Store có Override giá thì dùng, ko thì dùng BasePrice
                dto.DisplayPrice = storeData.PriceOverride ?? p.BasePrice;

                // Logic tính lại giá các Size (nếu Store có override base price, thì size cũng phải tịnh tiến theo nếu cần thiết)
                // Ở bài trước ta đã tính FinalPrice trong Mapper. 
                // Nếu Store override giá gốc, ta cần update lại FinalPrice của các Size trong DTO
                if (storeData.PriceOverride.HasValue)
                {
                    decimal newBasePrice = storeData.PriceOverride.Value;

                    foreach (var size in dto.ProductSizes)
                    {
                        // 1. Xác định phần "Cộng thêm" của size này
                        // Nếu có Override riêng thì dùng, không thì dùng Modifier chuẩn
                        decimal effectiveModifier = size.PriceOverride ?? size.SizeModifierPrice;   

                        // 2. Tính lại FinalPrice theo giá gốc mới của cửa hàng
                        size.FinalPrice = newBasePrice + effectiveModifier;
                    }
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> UpdateProductStatusAtStoreAsync(ProductStoreUpdateDto updateDto)
        {
            var productStoreRepo = _unitOfWork.Repository<ProductStore>();

            // Tìm bản ghi trong bảng trung gian
            var productStore = await productStoreRepo.GetFirstOrDefaultAsync(
                ps => ps.ProductId == updateDto.ProductId && ps.StoreId == updateDto.StoreId
            );

            if (productStore == null) return false;

            // Chỉ cập nhật Status
            productStore.Status = updateDto.Status;
            productStore.UpdatedAt = DateTime.UtcNow;

            productStoreRepo.Update(productStore);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}