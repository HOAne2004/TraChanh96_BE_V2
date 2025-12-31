using drinking_be.Dtos.CartDtos;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartReadDto>> GetMyCartAsync(int userId);
        Task<IEnumerable<CartReadDto>> AddItemToCartAsync(int userId, CartItemCreateDto itemDto);
        Task<IEnumerable<CartReadDto>> UpdateItemQuantityAsync(int userId, CartItemUpdateDto updateDto);
        Task<IEnumerable<CartReadDto>> RemoveItemFromCartAsync(int userId, long cartItemId);
        Task ClearCartAsync(int userId);
        Task<IEnumerable<CartReadDto>> ClearCartByStoreAsync(int userId, int storeId);
    }
}