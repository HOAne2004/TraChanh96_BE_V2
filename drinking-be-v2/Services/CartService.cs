using AutoMapper;
using drinking_be.Dtos.CartDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // ✅ Dùng để gọi hàm GetDescription()
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

        public async Task<CartReadDto> GetMyCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return MapCartToReadDto(cart);
        }

        public async Task<CartReadDto> AddItemToCartAsync(int userId, CartItemCreateDto itemDto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            // 1. Lấy Product & Size (Dùng Generic Repository)
            var productIds = new List<int> { itemDto.ProductId };
            if (itemDto.Toppings != null)
            {
                productIds.AddRange(itemDto.Toppings.Select(t => t.ProductId));
            }

            var products = await _unitOfWork.Repository<Product>().GetAllAsync(p => productIds.Contains(p.Id));
            var allProductsMap = products.ToDictionary(p => p.Id);

            if (!allProductsMap.ContainsKey(itemDto.ProductId))
                throw new Exception("Sản phẩm chính không tồn tại.");

            // SizeId trong DTO là short, Size.Id cũng là short -> OK
            var size = await _unitOfWork.Repository<Size>().GetByIdAsync(itemDto.SizeId);
            if (size == null) throw new Exception("Size không hợp lệ.");

            // 2. Tính giá tiền
            var mainProduct = allProductsMap[itemDto.ProductId];
            decimal basePrice = mainProduct.BasePrice;
            decimal sizeModifier = size.PriceModifier ?? 0; // ✅ Xử lý nullable decimal

            decimal itemUnitPrice = basePrice + sizeModifier;
            decimal itemTotalPrice = itemUnitPrice * itemDto.Quantity;

            // 3. Tạo CartItem (Món chính)
            var mainCartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                BasePrice = basePrice,
                FinalPrice = itemTotalPrice,

                SizeId = itemDto.SizeId,

                // ✅ Ép kiểu từ short (DTO) sang Enum (Entity)
                SugarLevel = itemDto.SugarLevelId.HasValue ? (SugarLevelEnum)itemDto.SugarLevelId.Value : null,
                IceLevel = itemDto.IceLevelId.HasValue ? (IceLevelEnum)itemDto.IceLevelId.Value : null,

                Note = itemDto.Note,
                ParentItemId = null
            };

            // 4. Xử lý Topping (Món phụ)
            if (itemDto.Toppings != null)
            {
                foreach (var toppingDto in itemDto.Toppings)
                {
                    if (!allProductsMap.ContainsKey(toppingDto.ProductId)) continue;

                    var toppingProduct = allProductsMap[toppingDto.ProductId];

                    int totalToppingQty = toppingDto.Quantity * itemDto.Quantity;
                    decimal toppingTotalPrice = toppingProduct.BasePrice * totalToppingQty;

                    mainCartItem.InverseParentItem.Add(new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = toppingDto.ProductId,
                        Quantity = totalToppingQty,
                        BasePrice = toppingProduct.BasePrice,
                        FinalPrice = toppingTotalPrice,
                        // ParentItem = mainCartItem (EF tự gán)
                    });
                }
            }

            await _unitOfWork.Repository<CartItem>().AddAsync(mainCartItem);
            await _unitOfWork.SaveChangesAsync();

            return await GetMyCartAsync(userId);
        }

        public async Task<CartReadDto> UpdateItemQuantityAsync(int userId, CartItemUpdateDto updateDto)
        {
            var cartItemRepo = _unitOfWork.Repository<CartItem>();

            // Lấy item cần sửa, include Size và Topping con
            var cartItem = await cartItemRepo.GetFirstOrDefaultAsync(
                filter: ci => ci.Id == updateDto.CartItemId && ci.Cart.UserId == userId,
                includeProperties: "Size,InverseParentItem"
            );

            if (cartItem == null) throw new Exception("Sản phẩm không tồn tại trong giỏ.");

            int oldQuantity = cartItem.Quantity;
            int newQuantity = updateDto.Quantity;

            if (newQuantity <= 0)
            {
                cartItemRepo.Delete(cartItem); // Xóa món chính -> Cascade xóa món con
            }
            else
            {
                // Cập nhật món chính
                decimal sizeModifier = cartItem.Size?.PriceModifier ?? 0;
                decimal unitPrice = cartItem.BasePrice + sizeModifier;

                cartItem.Quantity = newQuantity;
                cartItem.FinalPrice = unitPrice * newQuantity;
                cartItemRepo.Update(cartItem);

                // Cập nhật topping con theo tỉ lệ
                if (cartItem.InverseParentItem != null)
                {
                    foreach (var topping in cartItem.InverseParentItem)
                    {
                        int toppingUnitQty = oldQuantity > 0 ? topping.Quantity / oldQuantity : 1;
                        topping.Quantity = toppingUnitQty * newQuantity;
                        topping.FinalPrice = topping.BasePrice * topping.Quantity;
                        cartItemRepo.Update(topping);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return await GetMyCartAsync(userId);
        }

        public async Task<CartReadDto> RemoveItemFromCartAsync(int userId, long cartItemId)
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

            return await GetMyCartAsync(userId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cartItemRepo = _unitOfWork.Repository<CartItem>();

            var cart = await cartRepo.GetFirstOrDefaultAsync(
                filter: c => c.UserId == userId,
                includeProperties: "CartItems"
            );

            if (cart != null && cart.CartItems.Any())
            {
                cartItemRepo.DeleteRange(cart.CartItems);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        // --- PRIVATE HELPER ---

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cart = await cartRepo.GetFirstOrDefaultAsync(
                filter: c => c.UserId == userId,
                includeProperties: "CartItems,CartItems.Product,CartItems.Size,CartItems.InverseParentItem,CartItems.InverseParentItem.Product"
            );

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
                await cartRepo.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }
            return cart;
        }

        private CartReadDto MapCartToReadDto(Cart cart)
        {
            var dto = _mapper.Map<CartReadDto>(cart);
            dto.Items = new List<CartItemReadDto>();
            dto.TotalAmount = 0;

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

                    // ⭐ [CẬP NHẬT] Dùng Enum Extensions để lấy Description tiếng Việt
                    if (mainItem.SugarLevel.HasValue)
                    {
                        itemDto.SugarLabel = mainItem.SugarLevel.Value.GetDescription();
                    }

                    if (mainItem.IceLevel.HasValue)
                    {
                        itemDto.IceLabel = mainItem.IceLevel.Value.GetDescription();
                    }

                    // Map Topping
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

                    // Tính tổng tiền hiển thị
                    decimal itemTotal = mainItem.FinalPrice;
                    if (mainItem.InverseParentItem != null)
                    {
                        itemTotal += mainItem.InverseParentItem.Sum(t => t.FinalPrice);
                    }

                    dto.Items.Add(itemDto);
                    dto.TotalAmount += itemTotal;
                }
            }

            return dto;
        }
    }
}