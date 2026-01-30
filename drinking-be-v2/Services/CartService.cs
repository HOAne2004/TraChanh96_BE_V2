using AutoMapper;
using drinking_be.Dtos.CartDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            // 1. Xác định StoreId
            // ⭐ 1.1 CHECK BRAND – Product phải Active
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

            // ⭐ 1.2 CHECK STORE – Store có bán sản phẩm này không
            int targetStoreId = itemDto.StoreId;
            var productStore = await _unitOfWork.Repository<ProductStore>()
                .GetFirstOrDefaultAsync(ps => ps.ProductId == itemDto.ProductId && ps.StoreId == targetStoreId);

            if (productStore == null || productStore.Status != ProductStoreStatusEnum.Available)
            {
                throw new Exception("Sản phẩm này không khả dụng tại cửa hàng đã chọn.");
            }

            // 2. Tìm giỏ hàng CỤ THỂ của User tại Store đó
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                filter: c => c.UserId == userId && c.StoreId == targetStoreId,
                includeProperties: "CartItems,CartItems.InverseParentItem"
            );

            // 3. Nếu chưa có giỏ cho Store này -> Tạo mới
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
                    // ⚠️ Có khả năng cart đã được tạo bởi request khác
                    cart = await cartRepo.GetFirstOrDefaultAsync(
                        c => c.UserId == userId
                          && c.StoreId == targetStoreId
                          && c.Status == CartStatusEnum.Active,
                        includeProperties: "CartItems,CartItems.InverseParentItem"
                    );

                    if (cart == null)
                        throw; // Trường hợp cực hiếm – cho bubble lên
                }
            }


            // 4. Logic thêm/sửa sản phẩm
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
                // Cập nhật số lượng
                existingItem.Quantity += itemDto.Quantity;

                decimal toppingTotalPerUnit = existingItem.InverseParentItem.Sum(t => t.BasePrice * (t.Quantity / (existingItem.Quantity - itemDto.Quantity)));
                // Note: Logic chia ở trên có thể gây lỗi chia 0 nếu logic sai, nhưng tạm thời dùng cách đơn giản hơn:
                // Ta tính lại giá topping dựa trên itemDto gửi lên (vì topping giống hệt nhau)

                // Cách an toàn hơn: Tính lại từ đầu
                // decimal itemTotalWithTopping = baseUnitPrice + (itemDto.Toppings?.Sum(t => ...) ?? 0);
                // Nhưng để đơn giản, ta cứ update số lượng topping con theo tỉ lệ

                // Fix logic đơn giản:
                foreach (var topping in existingItem.InverseParentItem)
                {
                    // Giả sử topping lưu db là tổng số lượng.
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
                // Thêm mới
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

            // ✅ XÓA DÒNG GỌI HÀM AddOrUpdateItemLogic VÌ LOGIC ĐÃ NẰM Ở TRÊN

            await _unitOfWork.SaveChangesAsync();

            // Trả về danh sách giỏ hàng
            return await GetMyCartAsync(userId);
        }

        // 🟢 SỬA RETURN TYPE: IEnumerable<CartReadDto>
        public async Task<IEnumerable<CartReadDto>> UpdateItemQuantityAsync(int userId, CartItemUpdateDto updateDto)
        {
            var cartItemRepo = _unitOfWork.Repository<CartItem>();
            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                filter: ci => ci.Id == updateDto.CartItemId && ci.Cart.UserId == userId,
                includeProperties: "Size,InverseParentItem"
            );

            if (cartItem == null) throw new Exception("Sản phẩm không tồn tại trong giỏ.");

            int oldQuantity = cartItem.Quantity;
            int newQuantity = updateDto.Quantity;

            if (newQuantity <= 0)
            {
                cartItemRepo.Delete(cartItem);
            }
            else
            {
                decimal sizeModifier = cartItem.Size?.PriceModifier ?? 0;
                decimal unitPrice = cartItem.BasePrice + sizeModifier; // BasePrice này trong DB đã bao gồm giá gốc sp

                // Cập nhật món chính
                // Lưu ý: BasePrice trong DB của CartItem lúc Add đã lưu là (Giá SP + Giá Size).
                // Nếu muốn tính lại chuẩn xác cần lấy lại Product.BasePrice, nhưng tạm thời dùng logic này nếu BasePrice không đổi.

                // Tính lại FinalPrice món chính (chưa topping)
                // cartItem.FinalPrice = cartItem.BasePrice * newQuantity; // SAI nếu BasePrice chỉ là đơn giá. Đúng.

                // Update topping
                if (cartItem.InverseParentItem != null)
                {
                    foreach (var topping in cartItem.InverseParentItem)
                    {
                        int toppingPerItem = oldQuantity > 0 ? topping.Quantity / oldQuantity : 1;
                        topping.Quantity = toppingPerItem * newQuantity;
                        topping.FinalPrice = topping.BasePrice * topping.Quantity;
                        topping.UpdatedAt = DateTime.UtcNow;
                        cartItemRepo.Update(topping);
                    }
                }

                // Tính lại tổng FinalPrice (Chính + Toppings)
                decimal totalToppingPrice = cartItem.InverseParentItem?.Sum(t => t.FinalPrice) ?? 0;
                cartItem.FinalPrice = cartItem.BasePrice + totalToppingPrice; // BasePrice ở đây là đơn giá (đã cộng size)

                cartItem.Quantity = newQuantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                cartItemRepo.Update(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
            return await GetMyCartAsync(userId); // ✅ Hết lỗi CS0266
        }

        // 🟢 SỬA RETURN TYPE: IEnumerable<CartReadDto>
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