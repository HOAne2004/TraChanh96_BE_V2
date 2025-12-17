using drinking_be.Dtos.PolicyDtos;
using drinking_be.Enums;
using drinking_be.Interfaces.PolicyInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        // --- PUBLIC API ---

        [HttpGet("active")] // Route: /api/policies/active?brandId=1
        [AllowAnonymous]
        public async Task<IActionResult> GetActivePolicies([FromQuery] int brandId = 1) // Mặc định Brand 1
        {
            var policies = await _policyService.GetActivePoliciesAsync(brandId);
            return Ok(policies);
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var policy = await _policyService.GetPolicyBySlugAsync(slug);
            if (policy == null) return NotFound("Chính sách không tồn tại.");
            return Ok(policy);
        }

        // --- ADMIN API ---

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] int? brandId, [FromQuery] int? storeId, [FromQuery] PolicyReviewStatusEnum? status)
        {
            var result = await _policyService.GetAllPoliciesAsync(brandId, storeId, status);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] PolicyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _policyService.CreatePolicyAsync(dto);
                return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] PolicyUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _policyService.UpdatePolicyAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _policyService.DeletePolicyAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}