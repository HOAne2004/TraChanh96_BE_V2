// Interfaces/ICartService.cs
using drinking_be.Dtos.CartDtos;
using System.Threading.Tasks;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface ICartService
    {
        Task<CartReadDto> GetMyCartAsync(int userId);
        Task<CartReadDto> AddItemToCartAsync(int userId, CartItemCreateDto itemDto);
        Task<CartReadDto> UpdateItemQuantityAsync(int userId, CartItemUpdateDto updateDto);
        Task<CartReadDto> RemoveItemFromCartAsync(int userId, long cartItemId);
        Task ClearCartAsync(int userId);
    }
}