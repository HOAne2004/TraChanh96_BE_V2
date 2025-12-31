using drinking_be.Dtos.CartDtos;
using drinking_be.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Helper lấy UserID chuẩn
        private int GetUserIdFromToken()
        {
            return User.GetUserId();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyCart()
        {
            var carts = await _cartService.GetMyCartAsync(GetUserIdFromToken());
            return Ok(carts); // Trả về mảng []
        }
        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemCreateDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var cart = await _cartService.AddItemToCartAsync(GetUserIdFromToken(), itemDto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateItemQuantity([FromBody] CartItemUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var cart = await _cartService.UpdateItemQuantityAsync(GetUserIdFromToken(), updateDto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove-item/{cartItemId}")]
        public async Task<IActionResult> RemoveItemFromCart(long cartItemId)
        {
            var cart = await _cartService.RemoveItemFromCartAsync(GetUserIdFromToken(), cartItemId);
            return Ok(cart);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserIdFromToken());
            // Trả về giỏ rỗng
            var emptyCart = await _cartService.GetMyCartAsync(GetUserIdFromToken());
            return Ok(emptyCart);
        }

        [HttpDelete("clear/{storeId}")]
        public async Task<IActionResult> ClearCartByStore(int storeId)
        {
            var carts = await _cartService.ClearCartByStoreAsync(
                GetUserIdFromToken(),
                storeId
            );
            return Ok(carts);
        }

    }
}