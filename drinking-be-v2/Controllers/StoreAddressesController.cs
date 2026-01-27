using drinking_be.Dtos.AddressDtos;
using drinking_be.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/stores/addresses")]
[ApiController]
[Authorize(Roles = "Admin")]
public class StoreAddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public StoreAddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StoreAddressCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _addressService.CreateStoreAddressAsync(dto.StoreId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] StoreAddressCreateDto dto)
    {
        var result = await _addressService.UpdateStoreAddressAsync(id, dto.StoreId, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, [FromBody] StoreAddressCreateDto dto)
    {
        var ok = await _addressService.DeleteStoreAddressAsync(id, dto.StoreId);
        return ok ? NoContent() : NotFound();
    }
}
