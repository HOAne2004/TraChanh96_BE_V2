using AutoMapper;
using drinking_be.Dtos.StoreDtos;
using drinking_be.Dtos.ProductDtos; // Import namespace chứa ProductReadDto
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class StoreMenuService : IStoreMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreMenuService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StoreMenuReadDto>> GetMenuByStoreIdAsync(int storeId)
        {
            // 1. Kiểm tra Store Active
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store == null || store.Status != StoreStatusEnum.Active)
            {
                throw new Exception("Cửa hàng không tồn tại hoặc đang đóng cửa.");
            }

            // 2. Query ProductStore kết hợp Product + Category + Sizes
            // Lưu ý: Phải Include ProductSizes để FE có dữ liệu chọn Size
            var productStores = await _unitOfWork.Repository<ProductStore>()
                .GetQueryable()
                .Include(ps => ps.Product)
                    .ThenInclude(p => p.Category)
                .Include(ps => ps.Product)
                    .ThenInclude(p => p.ProductSizes) // Quan trọng: Lấy size
                        .ThenInclude(ps => ps.Size)
                .Where(ps =>
                    ps.StoreId == storeId &&
                    (ps.Status == ProductStoreStatusEnum.Available || ps.Status == ProductStoreStatusEnum.OutOfStock) && // Lấy cả OutOfStock để hiện mờ
                    ps.Product.Status == ProductStatusEnum.Active // Chỉ lấy món Brand còn bán
                )
                .OrderBy(ps => ps.Product.Category.SortOrder)
                .ThenBy(ps => ps.Product.Name)
                .ToListAsync();

            // 3. Mapping sang StoreMenuReadDto
            var menuList = new List<StoreMenuReadDto>();

            foreach (var ps in productStores)
            {
                // Sử dụng AutoMapper để map các trường cơ bản từ Product -> StoreMenuReadDto (vì nó kế thừa)
                // Yêu cầu: Bạn phải có Config Map: CreateMap<Product, StoreMenuReadDto>();
                var dto = _mapper.Map<StoreMenuReadDto>(ps.Product);

                // Ghi đè/Bổ sung thông tin riêng của Store
                dto.StoreStatus = ps.Status.ToString();
                dto.IsSoldOut = ps.Status == ProductStoreStatusEnum.OutOfStock;

                // Logic hiển thị giá: Nếu Store có giá riêng (PriceOverride) thì dùng, không thì dùng giá gốc
                dto.DisplayPrice = ps.PriceOverride ?? ps.Product.BasePrice;

                // Xử lý danh sách Size (nếu cần thiết map thủ công, nhưng thường AutoMapper lo được phần này từ Product)
                // dto.ProductSizes = ... (Đã được map từ dòng _mapper.Map ở trên)

                menuList.Add(dto);
            }

            return menuList;
        }
    }
}