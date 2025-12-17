using drinking_be.Dtos.StaffDtos;
using drinking_be.Interfaces.StoreInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Chỉ quản lý mới được vào
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: api/staffs?storeId=1&search=Huy
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] string? search)
        {
            var staffs = await _staffService.GetAllAsync(storeId, search);
            return Ok(staffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffCreateDto createDto)
        {
            try
            {
                var newStaff = await _staffService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = newStaff.Id }, newStaff);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffUpdateDto updateDto)
        {
            var updatedStaff = await _staffService.UpdateAsync(id, updateDto);
            if (updatedStaff == null) return NotFound();
            return Ok(updatedStaff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _staffService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}