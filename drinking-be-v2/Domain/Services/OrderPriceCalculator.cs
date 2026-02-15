using drinking_be.Dtos.OrderItemDtos;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Domain.Services
{
    public class OrderPriceCalculator
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderPriceCalculator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public class AppException : Exception
        {
            public AppException(string message) : base(message) { }
        }

        /// <summary>
        /// Xử lý danh sách món ăn: Validate, Tính giá, Tạo OrderItem entity
        /// </summary>
        /// <returns>Tổng tiền hàng (SubTotal)</returns>
        public async Task<decimal> CalculateAndCreateItemsAsync(Order order, List<OrderItemCreateDto> itemsDto)
        {
            decimal totalAmount = 0;

            if (itemsDto == null || !itemsDto.Any())
                throw new AppException("Danh sách món không được rỗng.");

            // 1. Gom tất cả ProductId (Món chính + Topping) để query 1 lần
            var productIds = itemsDto
                .Select(i => i.ProductId)
                .Concat(itemsDto.SelectMany(i => i.Toppings.Select(t => t.ProductId)))
                .Distinct()
                .ToList();

            // 2. Load Product từ DB (Kèm Size và Store để validate)
            var products = await _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductStores)
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // 3. Duyệt qua từng món khách đặt
            foreach (var itemDto in itemsDto)
            {
                if (itemDto.Quantity <= 0)
                    throw new AppException("Số lượng sản phẩm không hợp lệ.");

                // Validate Món chính
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                    throw new AppException($"Sản phẩm (ID: {itemDto.ProductId}) không tồn tại.");

                if (!product.ProductStores.Any(ps => ps.StoreId == order.StoreId))
                    throw new AppException($"Sản phẩm '{product.Name}' không thuộc cửa hàng này.");

                // --- BẮT ĐẦU TÍNH TIỀN ---
                var basePrice = product.BasePrice;
                var unitPrice = basePrice;

                short? sizeId = null;
                string? sizeName = null;
                decimal sizePrice = 0;

                // Xử lý Size
                if (itemDto.SizeId.HasValue)
                {
                    var size = product.ProductSizes.FirstOrDefault(s => s.SizeId == itemDto.SizeId.Value);
                    if (size == null)
                        throw new AppException($"Size không hợp lệ cho sản phẩm '{product.Name}'.");

                    sizePrice = size.PriceOverride.GetValueOrDefault(0);
                    unitPrice += sizePrice; // Cộng giá size vào đơn giá

                    sizeId = size.SizeId;
                    sizeName = size.Size?.Label; // Nếu Include Size ở trên thì mới có Label
                }

                // Tạo Entity Món chính
                var mainItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.ImageUrl,
                    Quantity = itemDto.Quantity,
                    BasePrice = basePrice,

                    // Snapshot thông tin Size
                    SizeId = sizeId,
                    SizeName = sizeName,
                    SizePrice = sizePrice,

                    SugarLevel = itemDto.SugarLevel,
                    IceLevel = itemDto.IceLevel,
                    Note = itemDto.Note,
                    Order = order // Link vào Order
                };

                // Xử lý Topping
                decimal toppingUnitTotal = 0;

                foreach (var toppingDto in itemDto.Toppings)
                {
                    if (!products.TryGetValue(toppingDto.ProductId, out var toppingProduct))
                        throw new AppException($"Topping (ID: {toppingDto.ProductId}) không tồn tại.");

                    if (!toppingProduct.ProductStores.Any(ps => ps.StoreId == order.StoreId))
                        throw new AppException($"Topping '{toppingProduct.Name}' không thuộc cửa hàng này.");

                    toppingUnitTotal += toppingProduct.BasePrice;

                    var toppingItem = new OrderItem
                    {
                        ProductId = toppingProduct.Id,
                        ProductName = toppingProduct.Name,
                        ProductImage = toppingProduct.ImageUrl,
                        Quantity = mainItem.Quantity, // Số lượng topping theo món chính
                        BasePrice = toppingProduct.BasePrice,
                        FinalPrice = toppingProduct.BasePrice * mainItem.Quantity,

                        ParentItem = mainItem, // Link vào món chính
                        Order = order
                    };

                    mainItem.InverseParentItem.Add(toppingItem);
                }

                // Tính FinalPrice cho món chính = (Giá gốc + Giá Size + Tổng Topping) * Số lượng
                mainItem.FinalPrice = (unitPrice + toppingUnitTotal) * mainItem.Quantity;

                // Cộng dồn vào tổng đơn
                totalAmount += mainItem.FinalPrice;

                // Thêm vào Order
                order.OrderItems.Add(mainItem);
            }

            return totalAmount;
        }
    }
}