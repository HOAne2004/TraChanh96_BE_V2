using AutoMapper;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Dtos.OrderItemDtos; // Import DTOs con
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Để dùng GetDescription()
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderReadDto> CreateOrderAsync(int userId, OrderCreateDto dto)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var sizeRepo = _unitOfWork.Repository<Size>();
            var addressRepo = _unitOfWork.Repository<Address>();
            var orderRepo = _unitOfWork.Repository<Order>();

            // 1. Validate Địa chỉ
            var address = await addressRepo.GetFirstOrDefaultAsync(a => a.Id == dto.DeliveryAddressId && a.UserId == userId);
            if (address == null) throw new Exception("Địa chỉ giao hàng không hợp lệ.");

            // 2. Lấy dữ liệu Sản phẩm & Size
            var allProductIds = dto.Items.Select(i => i.ProductId)
                .Concat(dto.Items.SelectMany(i => i.Toppings).Select(t => t.ProductId))
                .Distinct().ToList();

            var products = await productRepo.GetAllAsync(p => allProductIds.Contains(p.Id));
            var productMap = products.ToDictionary(p => p.Id);

            var sizeIds = dto.Items.Where(i => i.SizeId.HasValue).Select(i => i.SizeId!.Value).Distinct().ToList();
            var sizes = await sizeRepo.GetAllAsync(s => sizeIds.Contains(s.Id));
            var sizeMap = sizes.ToDictionary(s => s.Id);

            // 3. Khởi tạo Order
            var order = new Order
            {
                UserId = userId,
                StoreId = dto.StoreId,
                PaymentMethodId = dto.PaymentMethodId,
                DeliveryAddressId = dto.DeliveryAddressId,
                UserNotes = dto.UserNotes,
                VoucherCodeUsed = dto.VoucherCodeUsed,

                OrderCode = $"ORD-{DateTime.UtcNow.Ticks}",
                OrderDate = DateTime.UtcNow,
                Status = OrderStatusEnum.New,
                CreatedAt = DateTime.UtcNow, // Thêm CreatedAt nếu model có

                ShippingFee = 0,
                DiscountAmount = 0
            };

            decimal subTotal = 0;

            // 4. Tạo Order Items & Tính tiền
            foreach (var itemDto in dto.Items)
            {
                if (!productMap.ContainsKey(itemDto.ProductId)) throw new Exception($"Sản phẩm ID {itemDto.ProductId} không tồn tại.");

                var product = productMap[itemDto.ProductId];

                decimal basePrice = product.BasePrice;
                decimal sizeModifier = 0;

                if (itemDto.SizeId.HasValue)
                {
                    if (!sizeMap.ContainsKey(itemDto.SizeId.Value)) throw new Exception($"Size ID {itemDto.SizeId} không hợp lệ.");
                    sizeModifier = sizeMap[itemDto.SizeId.Value].PriceModifier ?? 0;
                }

                decimal itemUnitPrice = basePrice + sizeModifier;
                decimal itemFinalPrice = itemUnitPrice * itemDto.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    BasePrice = basePrice,
                    FinalPrice = itemFinalPrice,
                    SizeId = itemDto.SizeId,
                    Note = itemDto.Note,

                    // ✅ FIX LỖI 1: Xử lý Enum không nullable
                    // Nếu DTO có giá trị -> Ép kiểu về Enum
                    // Nếu DTO null -> Lấy giá trị mặc định của Enum (S100/I100) hoặc giá trị đầu tiên
                    SugarLevel = itemDto.SugarLevel.HasValue ? (SugarLevelEnum)itemDto.SugarLevel.Value : SugarLevelEnum.S100,
                    IceLevel = itemDto.IceLevel.HasValue ? (IceLevelEnum)itemDto.IceLevel.Value : IceLevelEnum.I100,
                };

                subTotal += itemFinalPrice;

                // 5. Xử lý Topping
                if (itemDto.Toppings != null && itemDto.Toppings.Any())
                {
                    foreach (var toppingDto in itemDto.Toppings)
                    {
                        if (!productMap.ContainsKey(toppingDto.ProductId)) continue;
                        var toppingProduct = productMap[toppingDto.ProductId];

                        int totalToppingQty = itemDto.Quantity * toppingDto.Quantity;
                        decimal toppingFinalPrice = toppingProduct.BasePrice * totalToppingQty;

                        // ✅ FIX LỖI 2: Đổi tên InverseParent -> InverseParentItem (Theo Model của bạn)
                        orderItem.InverseParentItem.Add(new OrderItem
                        {
                            ProductId = toppingDto.ProductId,
                            Quantity = totalToppingQty,
                            BasePrice = toppingProduct.BasePrice,
                            FinalPrice = toppingFinalPrice,
                            // Topping mặc định Sugar/Ice chuẩn
                            SugarLevel = SugarLevelEnum.S100,
                            IceLevel = IceLevelEnum.I100
                        });

                        subTotal += toppingFinalPrice;
                    }
                }

                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = subTotal;
            order.GrandTotal = subTotal + (order.ShippingFee ?? 0) - (order.DiscountAmount ?? 0);

            // 7. Lưu vào DB
            await orderRepo.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return (await GetOrderByIdAsync(order.Id))!;
        }

        public async Task<OrderReadDto?> GetOrderByIdAsync(long orderId)
        {
            var repo = _unitOfWork.Repository<Order>();

            // ✅ FIX LỖI 2: Đổi InverseParent -> InverseParentItem trong chuỗi Include
            var order = await repo.GetFirstOrDefaultAsync(
                filter: o => o.Id == orderId,
                includeProperties: "OrderItems,OrderItems.Product,OrderItems.Size,OrderItems.InverseParentItem,OrderItems.InverseParentItem.Product,DeliveryAddress,PaymentMethod,Store"
            );

            if (order == null) return null;

            return MapOrderToDto(order);
        }

        public async Task<IEnumerable<OrderReadDto>> GetMyOrdersAsync(int userId, OrderStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Order>();

            // ✅ FIX LỖI 2: Đổi tên trong Include
            var query = await repo.GetAllAsync(
                filter: o => o.UserId == userId && (!status.HasValue || o.Status == status.Value),
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                includeProperties: "OrderItems,OrderItems.Product,OrderItems.Size,OrderItems.InverseParentItem,OrderItems.InverseParentItem.Product,DeliveryAddress,PaymentMethod,Store"
            );

            return query.Select(MapOrderToDto);
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync(OrderStatusEnum? status, string? searchCode)
        {
            var repo = _unitOfWork.Repository<Order>();

            var query = await repo.GetAllAsync(
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                includeProperties: "OrderItems,OrderItems.Product,OrderItems.Size,DeliveryAddress,PaymentMethod,Store"
            );

            if (status.HasValue) query = query.Where(o => o.Status == status.Value);
            if (!string.IsNullOrEmpty(searchCode)) query = query.Where(o => o.OrderCode.Contains(searchCode));

            return query.Select(MapOrderToDto);
        }

        public async Task<OrderReadDto?> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus)
        {
            var repo = _unitOfWork.Repository<Order>();
            var order = await repo.GetByIdAsync(orderId);

            if (order == null) return null;

            order.Status = newStatus;

            // ✅ FIX LỖI 3: Model Order chưa có UpdatedAt, tạm thời bỏ qua hoặc thêm vào Model
            // order.UpdatedAt = DateTime.UtcNow; 

            repo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return MapOrderToDto(order);
        }

        public async Task<bool> CancelOrderAsync(long orderId, int userId, string reason)
        {
            var repo = _unitOfWork.Repository<Order>();
            var order = await repo.GetFirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null) return false;

            if (order.Status != OrderStatusEnum.New)
            {
                throw new Exception("Không thể hủy đơn hàng đã được xác nhận hoặc đang giao.");
            }

            order.Status = OrderStatusEnum.Cancelled;
            order.UserNotes = string.IsNullOrEmpty(order.UserNotes) ? $"[Lý do hủy: {reason}]" : $"{order.UserNotes} | [Lý do hủy: {reason}]";

            // ✅ FIX LỖI 3: Bỏ cập nhật UpdatedAt nếu không có
            // order.UpdatedAt = DateTime.UtcNow;

            repo.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private OrderReadDto MapOrderToDto(Order order)
        {
            var dto = _mapper.Map<OrderReadDto>(order);
            dto.Items = new List<OrderItemReadDto>();

            // ✅ FIX LỖI 2: Lọc mainItems và map Topping từ InverseParentItem
            if (order.OrderItems != null)
            {
                var mainItems = order.OrderItems.Where(i => i.ParentItemId == null).ToList();

                foreach (var item in mainItems)
                {
                    var itemDto = _mapper.Map<OrderItemReadDto>(item);

                    if (item.Product != null) itemDto.ProductName = item.Product.Name;
                    if (item.Size != null) itemDto.SizeLabel = item.Size.Label;

                    // Enum không null -> gọi GetDescription() trực tiếp
                    itemDto.SugarLabel = item.SugarLevel.GetDescription();
                    itemDto.IceLabel = item.IceLevel.GetDescription();

                    // Map Topping từ InverseParentItem
                    if (item.InverseParentItem != null && item.InverseParentItem.Any())
                    {
                        itemDto.Toppings = item.InverseParentItem.Select(t => new OrderToppingReadDto
                        {
                            Id = t.Id,
                            ProductId = t.ProductId,
                            ProductName = t.Product?.Name ?? "Topping",
                            Quantity = t.Quantity,
                            BasePrice = t.BasePrice,
                            FinalPrice = t.FinalPrice
                        }).ToList();
                    }

                    dto.Items.Add(itemDto);
                }
            }

            return dto;
        }
    }
}