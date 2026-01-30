using drinking_be.Dtos.StoreDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IStoreMenuService _storeMenuService;
        public StoreController(IStoreService storeService, IStoreMenuService storeMenuService)
        {
            _storeService = storeService;
            _storeMenuService = storeMenuService;
        }

        // --- PUBLIC ---

        [HttpGet]
        public async Task<IActionResult> GetActiveStores()
        {
            var stores = await _storeService.GetActiveStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound("Cửa hàng không tồn tại.");
            return Ok(store);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetStoreBySlug(string slug)
        {
            var store = await _storeService.GetStoreBySlugAsync(slug);
            if (store == null) return NotFound("Cửa hàng không tồn tại.");
            return Ok(store);
        }

        // --- ADMIN ---

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] string? search, [FromQuery] StoreStatusEnum? status)
        {
            var result = await _storeService.GetAllStoresAsync(search, status);
            return Ok(result);
        }

        // Admin detail endpoint (returns store regardless of status)
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetByIdAdmin(int id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound("Cửa hàng không tồn tại.");
            return Ok(store);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] StoreCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _storeService.CreateStoreAsync(dto);
                return CreatedAtAction(nameof(GetStoreBySlug), new { slug = result.Slug }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] StoreUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _storeService.UpdateStoreAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _storeService.DeleteStoreAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/menu")]
        [AllowAnonymous] // Khách vãng lai cũng xem được menu
        public async Task<IActionResult> GetStoreMenu(int id)
        {
            try
            {
                var menu = await _storeMenuService.GetMenuByStoreIdAsync(id);
                return Ok(menu);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}