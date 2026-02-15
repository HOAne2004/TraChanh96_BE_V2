using drinking_be.Dtos.AddressDtos;
using drinking_be.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/users/addresses")]
[ApiController]
[Authorize]
public class UserAddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public UserAddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    private int GetUserId() => User.GetUserId();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _addressService.GetAllMyAddressesAsync(GetUserId()));

    /// <summary>
    /// [ADMIN] Lấy danh sách địa chỉ của khách hàng theo UserID
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin, Manager")] // Chỉ Admin/Manager được phép gọi
    public async Task<IActionResult> GetByUserId(int userId)
    {
        // Tái sử dụng hàm GetAllMyAddressesAsync của Service (vì nó nhận userId int)
        var result = await _addressService.GetAllMyAddressesAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _addressService.GetByIdAsync(id, GetUserId());
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddressCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _addressService.CreateUserAddressAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UserAddressUpdateDto dto)
    {
        var result = await _addressService.UpdateUserAddressAsync(id, GetUserId(), dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _addressService.DeleteUserAddressAsync(id, GetUserId());
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/set-default")]
    public async Task<IActionResult> SetDefault(long id)
    {
        var ok = await _addressService.SetDefaultAddressAsync(id, GetUserId());
        return ok ? Ok() : NotFound();
    }
}
