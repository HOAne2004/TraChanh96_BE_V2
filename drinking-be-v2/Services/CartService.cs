using AutoMapper;
using drinking_be.Dtos.CartDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;
using static drinking_be.Services.OrderService;

namespace drinking_be.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
        }

        // --- PUBLIC METHODS ---

        public async Task<IEnumerable<CartReadDto>> GetMyCartAsync(int userId)
        {
            var cartRepo = _unitOfWork.Repository<Cart>();

            // Lấy TẤT CẢ giỏ hàng của user
            var carts = await cartRepo.GetAllAsync(
                filter: c => c.UserId == userId && c.Status == CartStatusEnum.Active,
                includeProperties: "Store,CartItems,CartItems.Product,CartItems.Size,CartItems.InverseParentItem,CartItems.InverseParentItem.Product"
            );

            var result = new List<CartReadDto>();
            foreach (var cart in carts)
            {
                // Chỉ hiển thị những giỏ có sản phẩm
                if (cart.CartItems != null && cart.CartItems.Any())
                {
                    result.Add(MapCartToReadDto(cart));
                }
            }
            return result;
        }

        public async Task<IEnumerable<CartReadDto>> AddItemToCartAsync(int userId, CartItemCreateDto itemDto)
        {
            // 1. Lấy cấu hình động (truyền số 20 làm mặc định dự phòng nếu lỗi)
            int maxQtyPerItem = await _settingService.GetIntValueAsync("MaxQuantityPerItem", 20);
            int maxTotalItems = await _settingService.GetIntValueAsync("MaxTotalItemsPerOrder", 50); // Thêm dòng này

            // 2. Kiểm tra
            if (itemDto.Quantity > maxQtyPerItem)
            {
                throw new AppException($"Hệ thống hiện chỉ cho phép đặt tối đa {maxQtyPerItem} ly cho mỗi món.");
            }

            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(itemDto.ProductId);

            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            if (product.Status != ProductStatusEnum.Active)
            {
                throw new Exception("Sản phẩm hiện không được bán.");
            }

            int targetStoreId = itemDto.StoreId;
            var productStore = await _unitOfWork.Repository<ProductStore>()
                .GetFirstOrDefaultAsync(ps => ps.ProductId == itemDto.ProductId && ps.StoreId == targetStoreId);

            if (productStore == null || productStore.Status != ProductStoreStatusEnum.Available)
            {
                throw new Exception("Sản phẩm này không khả dụng tại cửa hàng đã chọn.");
            }

            var cartRepo = _unitOfWork.Repository<Cart>();
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                filter: c => c.UserId == userId && c.StoreId == targetStoreId,
                includeProperties: "CartItems,CartItems.InverseParentItem"
            );

            // SAU KHI LẤY ĐƯỢC CART (Trước nhánh xử lý thêm/sửa sản phẩm):
            if (cart != null)
            {
                // Kiểm tra tổng số lượng hiện tại trong giỏ
                int currentTotalQuantity = cart.CartItems.Where(i => i.ParentItemId == null).Sum(i => i.Quantity);
                if (currentTotalQuantity + itemDto.Quantity > maxTotalItems)
                {
                    throw new AppException($"Giỏ hàng chỉ chứa tối đa {maxTotalItems} sản phẩm. Bạn hiện có {currentTotalQuantity} sản phẩm.");
                }
            }

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    StoreId = targetStoreId,
                    Status = CartStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await cartRepo.AddAsync(cart);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    cart = await cartRepo.GetFirstOrDefaultAsync(
                        c => c.UserId == userId
                          && c.StoreId == targetStoreId
                          && c.Status == CartStatusEnum.Active,
                        includeProperties: "CartItems,CartItems.InverseParentItem"
                    );

                    if (cart == null)
                        throw; 
                }
            }

            var products = await _unitOfWork.Repository<Product>().GetAllAsync(p => p.Id == itemDto.ProductId);
            var mainProduct = products.FirstOrDefault();
            if (mainProduct == null) throw new Exception("Sản phẩm không tồn tại.");

            Size? size = null;
            if (itemDto.SizeId.HasValue)
            {
                size = await _unitOfWork.Repository<Size>().GetByIdAsync(itemDto.SizeId.Value);
            }

            decimal sizeModifier = size?.PriceModifier ?? 0;
            decimal productBasePrice = productStore.PriceOverride ?? product.BasePrice;
            decimal baseUnitPrice = productBasePrice + sizeModifier;


            var sugarEnum = itemDto.SugarLevelId.HasValue ? (SugarLevelEnum)itemDto.SugarLevelId.Value : (SugarLevelEnum?)null;
            var iceEnum = itemDto.IceLevelId.HasValue ? (IceLevelEnum)itemDto.IceLevelId.Value : (IceLevelEnum?)null;

            var existingItem = cart.CartItems.FirstOrDefault(i =>
                i.ParentItemId == null &&
                i.ProductId == itemDto.ProductId &&
                i.SizeId == itemDto.SizeId &&
                i.SugarLevel == sugarEnum &&
                i.IceLevel == iceEnum &&
                string.Equals(i.Note, itemDto.Note) &&
                AreToppingsEqual(i.InverseParentItem, itemDto.Toppings)
            );

            if (existingItem != null)
            {
                // Chặn cộng dồn vượt quá Max Quantity
                if (existingItem.Quantity + itemDto.Quantity > maxQtyPerItem)
                {
                    throw new AppException($"Món này đã có {existingItem.Quantity} ly trong giỏ. Tối đa chỉ được {maxQtyPerItem} ly/món.");
                }

                // Cập nhật số lượng
                existingItem.Quantity += itemDto.Quantity;

                decimal toppingTotalPerUnit = existingItem.InverseParentItem.Sum(t => t.BasePrice * (t.Quantity / (existingItem.Quantity - itemDto.Quantity)));
                foreach (var topping in existingItem.InverseParentItem)
                {
                    // Tỷ lệ = topping.Quantity hiện tại / (Quantity cũ của parent)
                    int oldParentQty = existingItem.Quantity - itemDto.Quantity;
                    int qtyPerUnit = oldParentQty > 0 ? topping.Quantity / oldParentQty : 1;

                    topping.Quantity = qtyPerUnit * existingItem.Quantity;
                    topping.FinalPrice = topping.BasePrice * topping.Quantity;
                    topping.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<CartItem>().Update(topping);
                }

                decimal currentToppingPrice = existingItem.InverseParentItem.Sum(t => t.FinalPrice);
                existingItem.FinalPrice = (baseUnitPrice * existingItem.Quantity) + currentToppingPrice;

                existingItem.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<CartItem>().Update(existingItem);
            }
            else
            {
                decimal itemTotalPrice = baseUnitPrice * itemDto.Quantity;

                var mainCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    BasePrice = baseUnitPrice,
                    FinalPrice = itemTotalPrice,
                    SizeId = itemDto.SizeId,
                    SugarLevel = sugarEnum,
                    IceLevel = iceEnum,
                    Note = itemDto.Note,
                    ParentItemId = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (itemDto.Toppings != null && itemDto.Toppings.Any())
                {
                    var toppingIds = itemDto.Toppings.Select(t => t.ProductId).ToList();
                    var toppingProducts = await _unitOfWork.Repository<Product>().GetAllAsync(p => toppingIds.Contains(p.Id));

                    foreach (var toppingDto in itemDto.Toppings)
                    {
                        var tp = toppingProducts.FirstOrDefault(p => p.Id == toppingDto.ProductId);
                        if (tp == null) continue;

                        int totalToppingQty = toppingDto.Quantity * itemDto.Quantity;
                        decimal toppingPrice = tp.BasePrice * totalToppingQty;

                        mainCartItem.InverseParentItem.Add(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = toppingDto.ProductId,
                            Quantity = totalToppingQty,
                            BasePrice = tp.BasePrice,
                            FinalPrice = toppingPrice,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });

                        mainCartItem.FinalPrice += toppingPrice;
                    }
                }

                await _unitOfWork.Repository<CartItem>().AddAsync(mainCartItem);
            }

            await _unitOfWork.SaveChangesAsync();
            return await GetMyCartAsync(userId);
        }

        public async Task<IEnumerable<CartReadDto>> UpdateCartItemAsync(int userId, CartItemUpdateDto updateDto)
        {
            var cartItemRepo = _unitOfWork.Repository<CartItem>();

            // 1. Query lấy item (Cần Include Product để lấy giá gốc tính toán lại)
            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                filter: ci => ci.Id == updateDto.CartItemId && ci.Cart.UserId == userId,
                includeProperties: "Cart,Product,Size,InverseParentItem"
            );

            if (cartItem == null) throw new Exception("Sản phẩm không tồn tại trong giỏ.");

            // ==========================================
            // 2. XỬ LÝ XÓA MÓN NẾU QUANTITY == 0
            // ==========================================
            if (updateDto.Quantity <= 0)
            {
                // Xóa các topping con trước (tránh lỗi khóa ngoại nếu DB không bật Cascade Delete)
                if (cartItem.InverseParentItem != null && cartItem.InverseParentItem.Any())
                {
                    cartItemRepo.DeleteRange(cartItem.InverseParentItem);
                }

                cartItemRepo.Delete(cartItem);
                await _unitOfWork.SaveChangesAsync();
                return await GetMyCartAsync(userId);
            }

            int maxQtyPerItem = await _settingService.GetIntValueAsync("MaxQuantityPerItem", 20);
            int maxTotalItems = await _settingService.GetIntValueAsync("MaxTotalItemsPerOrder", 50);

            if (updateDto.Quantity > maxQtyPerItem)
                throw new AppException($"Hệ thống hiện chỉ cho phép đặt tối đa {maxQtyPerItem} ly cho mỗi món.");

            int qtyDifference = updateDto.Quantity - cartItem.Quantity;
            if (qtyDifference > 0)
            {
                // Phải query tổng số lượng hiện có trong cùng 1 giỏ hàng
                var allItemsInCart = await cartItemRepo.GetAllAsync(ci => ci.CartId == cartItem.CartId && ci.ParentItemId == null);
                int currentTotalQuantity = allItemsInCart.Sum(i => i.Quantity);

                if (currentTotalQuantity + qtyDifference > maxTotalItems)
                    throw new AppException($"Giỏ hàng chỉ chứa tối đa {maxTotalItems} sản phẩm. Tăng thêm sẽ vượt giới hạn.");
            }

            // ==========================================
            // 3. CẬP NHẬT CÁC THUỘC TÍNH CƠ BẢN
            // ==========================================
            if (updateDto.SizeId.HasValue && updateDto.SizeId != cartItem.SizeId)
            {
                // A. Lấy PriceModifier của Size mới
                var newSize = await _unitOfWork.Repository<Size>().GetByIdAsync(updateDto.SizeId.Value);
                decimal sizeModifier = newSize?.PriceModifier ?? 0;

                // B. Lấy PriceOverride của Store (phòng trường hợp giá tại quầy khác giá web)
                var productStore = await _unitOfWork.Repository<ProductStore>()
                    .GetFirstOrDefaultAsync(ps => ps.ProductId == cartItem.ProductId && ps.StoreId == cartItem.Cart.StoreId);

                decimal productBasePrice = productStore?.PriceOverride ?? cartItem.Product.BasePrice;

                // C. Cập nhật lại BasePrice cho CartItem (Giá 1 ly đã bao gồm size, chưa topping)
                cartItem.BasePrice = productBasePrice + sizeModifier;
                cartItem.SizeId = updateDto.SizeId.Value;
            }

            cartItem.Quantity = updateDto.Quantity;
            if (updateDto.SugarLevelId.HasValue) cartItem.SugarLevel = (SugarLevelEnum)updateDto.SugarLevelId.Value;
            if (updateDto.IceLevelId.HasValue) cartItem.IceLevel = (IceLevelEnum)updateDto.IceLevelId.Value;
            if (updateDto.Note != null) cartItem.Note = updateDto.Note;

            // ==========================================
            // 4. XỬ LÝ TOPPING MỚI
            // ==========================================
            // A. Dọn dẹp topping cũ (Giữ nguyên logic của bạn)
            if (cartItem.InverseParentItem != null && cartItem.InverseParentItem.Any())
            {
                cartItemRepo.DeleteRange(cartItem.InverseParentItem);
                cartItem.InverseParentItem.Clear();
            }

            decimal totalToppingPricePerUnit = 0;

            // B. Thêm topping mới và tính tổng tiền topping cho MỘT đơn vị sản phẩm
            if (updateDto.Toppings != null && updateDto.Toppings.Any())
            {
                var toppingIds = updateDto.Toppings.Select(t => t.ProductId).ToList();
                var toppingProducts = await _unitOfWork.Repository<Product>().GetAllAsync(p => toppingIds.Contains(p.Id));

                foreach (var toppingDto in updateDto.Toppings)
                {
                    var tp = toppingProducts.FirstOrDefault(p => p.Id == toppingDto.ProductId);
                    if (tp == null) continue;

                    // Lưu ý: Trong CartItem (topping), Quantity thường là số lượng cho mỗi ly món chính
                    totalToppingPricePerUnit += tp.BasePrice * toppingDto.Quantity;

                    cartItem.InverseParentItem.Add(new CartItem
                    {
                        CartId = cartItem.CartId,
                        ProductId = toppingDto.ProductId,
                        Quantity = toppingDto.Quantity * cartItem.Quantity, // Tổng số lượng topping = Qty mỗi ly * Tổng số ly
                        BasePrice = tp.BasePrice,
                        FinalPrice = tp.BasePrice * (toppingDto.Quantity * cartItem.Quantity),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            // Công thức: FinalPrice = (Giá món đã kèm size + Tổng tiền các topping của 1 ly) * Số lượng ly
            cartItem.FinalPrice = (cartItem.BasePrice + totalToppingPricePerUnit) * cartItem.Quantity;

            cartItem.UpdatedAt = DateTime.UtcNow;
            cartItemRepo.Update(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return await GetMyCartAsync(userId);
        }
        public async Task<IEnumerable<CartReadDto>> RemoveItemFromCartAsync(int userId, long cartItemId)
        {
            var cartItemRepo = _unitOfWork.Repository<CartItem>();
            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                ci => ci.Id == cartItemId && ci.Cart.UserId == userId
            );

            if (cartItem != null)
            {
                cartItemRepo.Delete(cartItem);
                await _unitOfWork.SaveChangesAsync();
            }
            return await GetMyCartAsync(userId); // ✅ Hết lỗi CS0266
        }

        public async Task ClearCartAsync(int userId)
        {
            // Clear all carts of user? Or specific cart?
            // Hiện tại clear all cho đơn giản
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cartItemRepo = _unitOfWork.Repository<CartItem>();

            var carts = await cartRepo.GetAllAsync(
                filter: c => c.UserId == userId,
                includeProperties: "CartItems"
            );

            foreach (var cart in carts)
            {
                if (cart.CartItems.Any())
                {
                    cartItemRepo.DeleteRange(cart.CartItems);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartReadDto>> ClearCartByStoreAsync(int userId, int storeId)
        {
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cartItemRepo = _unitOfWork.Repository<CartItem>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                c => c.UserId == userId
                  && c.StoreId == storeId
                  && c.Status == CartStatusEnum.Active,
                includeProperties: "CartItems"
            );

            if (cart != null && cart.CartItems.Any())
            {
                cartItemRepo.DeleteRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                cartRepo.Update(cart);

                await _unitOfWork.SaveChangesAsync();
            }

            // Trả về danh sách cart còn lại
            return await GetMyCartAsync(userId);
        }

        // --- HELPER ---
        private bool AreToppingsEqual(ICollection<CartItem> existingToppings, List<CartToppingCreateDto>? newToppings)
        {
            if ((existingToppings == null || !existingToppings.Any()) &&
                (newToppings == null || !newToppings.Any())) return true;

            if ((existingToppings == null || !existingToppings.Any()) ||
                (newToppings == null || !newToppings.Any())) return false;

            if (existingToppings.Count != newToppings.Count) return false;

            var existingList = existingToppings.Select(t => $"{t.ProductId}").OrderBy(x => x).ToList();
            var newList = newToppings.Select(t => $"{t.ProductId}").OrderBy(x => x).ToList();

            return Enumerable.SequenceEqual(existingList, newList);
        }

        private CartReadDto MapCartToReadDto(Cart cart)
        {
            var dto = _mapper.Map<CartReadDto>(cart);
            dto.Items = new List<CartItemReadDto>();
            dto.TotalAmount = 0;
            dto.StoreId = cart.StoreId;
            dto.StoreName = cart.Store?.Name ?? "Cửa hàng đang đóng";

            if (cart.CartItems != null)
            {
                var mainItems = cart.CartItems.Where(ci => ci.ParentItemId == null).ToList();

                foreach (var mainItem in mainItems)
                {
                    var itemDto = _mapper.Map<CartItemReadDto>(mainItem);

                    if (mainItem.Product != null)
                    {
                        itemDto.ProductName = mainItem.Product.Name;
                        itemDto.ProductSlug = mainItem.Product.Slug;
                        itemDto.ImageUrl = mainItem.Product.ImageUrl;
                    }
                    if (mainItem.Size != null) itemDto.SizeLabel = mainItem.Size.Label;

                    if (mainItem.SugarLevel.HasValue)
                    {
                        itemDto.SugarLevelId = (short)mainItem.SugarLevel.Value;
                        itemDto.SugarLabel = mainItem.SugarLevel.Value.GetDescription();
                    }

                    if (mainItem.IceLevel.HasValue)
                    {
                        itemDto.IceLevelId = (short)mainItem.IceLevel.Value;
                        itemDto.IceLabel = mainItem.IceLevel.Value.GetDescription();
                    }

                    if (mainItem.InverseParentItem != null && mainItem.InverseParentItem.Any())
                    {
                        itemDto.Toppings = mainItem.InverseParentItem.Select(t => new CartToppingReadDto
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

                    // TÍNH TỔNG: finalPrice của item + tổng finalPrice của toppings
                    decimal itemTotal = mainItem.FinalPrice;

                    if (mainItem.InverseParentItem != null && mainItem.InverseParentItem.Any())
                    {
                        itemTotal += mainItem.InverseParentItem.Sum(t => t.FinalPrice);
                    }

                    dto.TotalAmount += itemTotal * mainItem.Quantity;
                }
            }
            return dto;
        }
    }
}