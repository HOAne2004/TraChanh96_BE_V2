using drinking_be.Dtos.ProductDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;

namespace drinking_be.Services
{
    public class ProductStoreProvisionService : IProductStoreProvisionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductStoreProvisionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task InitializeProductStoresForNewStoreAsync(Store store)
        {
            // 1. Lấy toàn bộ product ACTIVE của Brand
            var brandProducts = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p =>
                    p.BrandId == store.BrandId &&
                    p.Status == ProductStatusEnum.Active
                );

            if (!brandProducts.Any()) return;

            // 2. Tạo ProductStore mặc định
            foreach (var product in brandProducts)
            {
                await _unitOfWork.Repository<ProductStore>().AddAsync(new ProductStore
                {
                    StoreId = store.Id,
                    ProductId = product.Id,

                    // ⭐ MẶC ĐỊNH STORE KHÔNG BÁN
                    Status = ProductStoreStatusEnum.Disabled,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EnableAsync(int brandId, int storeId, int productId)
        {
            // 1. Validate Product (Giữ nguyên)
            var product = await _unitOfWork.Repository<Product>()
                .GetFirstOrDefaultAsync(p =>
                    p.Id == productId &&
                    p.BrandId == brandId &&
                    p.Status == ProductStatusEnum.Active
                );

            if (product == null)
                throw new Exception("Sản phẩm không hợp lệ hoặc không thuộc brand.");

            // 2. Tìm ProductStore
            var psRepo = _unitOfWork.Repository<ProductStore>();
            var productStore = await psRepo.GetFirstOrDefaultAsync(ps =>
                ps.ProductId == productId &&
                ps.StoreId == storeId
            );

            // 🟢 SỬA: Nếu chưa có -> Tạo mới (Insert)
            if (productStore == null)
            {
                productStore = new ProductStore
                {
                    StoreId = storeId,
                    ProductId = productId,
                    Status = ProductStoreStatusEnum.Available, // Bật luôn
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await psRepo.AddAsync(productStore);
            }
            else
            {
                // 🟡 Nếu có rồi -> Cập nhật (Update)
                productStore.Status = ProductStoreStatusEnum.Available;
                productStore.UpdatedAt = DateTime.UtcNow;
                psRepo.Update(productStore);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DisableAsync(int brandId, int storeId, int productId)
        {
            var psRepo = _unitOfWork.Repository<ProductStore>();
            var productStore = await psRepo.GetFirstOrDefaultAsync(ps =>
                ps.ProductId == productId &&
                ps.StoreId == storeId
            );

            // 🟢 SỬA: Nếu chưa có -> Tạo mới với trạng thái Disabled (để lưu vào DB)
            // (Hoặc có thể return luôn nếu bạn muốn tiết kiệm DB, nhưng tạo mới sẽ chặt chẽ hơn cho các logic sau này)
            if (productStore == null)
            {
                productStore = new ProductStore
                {
                    StoreId = storeId,
                    ProductId = productId,
                    Status = ProductStoreStatusEnum.Disabled,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await psRepo.AddAsync(productStore);
            }
            else
            {
                // 🟡 Update
                productStore.Status = ProductStoreStatusEnum.Disabled;
                productStore.UpdatedAt = DateTime.UtcNow;
                psRepo.Update(productStore);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<ProductStoreAdminReadDto>> GetProductsByStoreAsync(int brandId, int storeId)
        {
            // 1️⃣ Validate Store thuộc Brand
            var store = await _unitOfWork.Repository<Store>()
                .GetFirstOrDefaultAsync(s => s.Id == storeId && s.BrandId == brandId);

            if (store == null)
                throw new Exception("Store không tồn tại hoặc không thuộc Brand.");

            // 2️⃣ Lấy ProductStore + Product
            var productStores = await _unitOfWork.Repository<ProductStore>()
                .GetAllAsync(
                    ps => ps.StoreId == storeId,
                    includeProperties: "Product"
                );

            // 3️⃣ Map ra DTO
            return productStores.Select(ps => new ProductStoreAdminReadDto
            {
                ProductId = ps.ProductId,
                ProductName = ps.Product.Name,
                BasePrice = ps.Product.BasePrice,
                Status = ps.Status
            });
        }
    }

}
