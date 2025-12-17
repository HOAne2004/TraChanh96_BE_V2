using drinking_be.Dtos.ShopTableDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopTableController : ControllerBase
    {
        private readonly IShopTableService _shopTableService;

        public ShopTableController(IShopTableService shopTableService)
        {
            _shopTableService = shopTableService;
        }

        // GET: api/shoptable/store/{storeId}?roomId=1
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetTablesByStore(int storeId, [FromQuery] int? roomId)
        {
            var tables = await _shopTableService.GetTablesByStoreAsync(storeId, roomId);
            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            var table = await _shopTableService.GetTableByIdAsync(id);
            if (table == null) return NotFound(new { message = "Không tìm thấy bàn." });
            return Ok(table);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTable([FromBody] ShopTableCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdTable = await _shopTableService.CreateTableAsync(createDto);
                return CreatedAtAction(nameof(GetTableById), new { id = createdTable.Id }, createdTable);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] ShopTableUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _shopTableService.UpdateTableAsync(id, updateDto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var result = await _shopTableService.DeleteTableAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}