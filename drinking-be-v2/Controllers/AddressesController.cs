using drinking_be.Dtos.AddressDtos;
using drinking_be.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/users/addresses")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Helper lấy UserID từ Token
        private int GetUserId()
        {
            return User.GetUserId();
        }

        // GET: api/users/addresses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _addressService.GetAllMyAddressesAsync(GetUserId());
                return Ok(result);
            }
            catch (Exception ex) { return Unauthorized(ex.Message); }
        }

        // GET: api/users/addresses/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _addressService.GetByIdAsync(id, GetUserId());
            if (result == null) return NotFound("Không tìm thấy địa chỉ.");
            return Ok(result);
        }

        // POST: api/users/addresses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _addressService.CreateAddressAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/users/addresses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AddressUpdateDto dto)
        {
            var result = await _addressService.UpdateAddressAsync(id, GetUserId(), dto);
            if (result == null) return NotFound("Không tìm thấy địa chỉ hoặc địa chỉ đã bị xóa.");
            return Ok(result);
        }

        // DELETE: api/users/addresses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _addressService.DeleteAddressAsync(id, GetUserId());
            if (!result) return NotFound();
            return NoContent();
        }

        // PATCH: api/users/addresses/{id}/set-default
        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(long id)
        {
            var result = await _addressService.SetDefaultAddressAsync(id, GetUserId());
            if (!result) return NotFound();
            return Ok(new { message = "Đã đặt làm địa chỉ mặc định thành công." });
        }
    }
}