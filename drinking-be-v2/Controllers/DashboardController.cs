using drinking_be.Dtos;
using drinking_be.Dtos.GlobalDtos;
using drinking_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ISystemAnalyticsService _analyticsService;

        public DashboardController(ISystemAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("global-stats")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được xem
        public async Task<IActionResult> GetGlobalStats([FromQuery] string timeframe = "today")
        {
            var stats = await _analyticsService.GetGlobalStatsAsync(timeframe);
            return Ok(new ApiResponse<GlobalDashboardStatsDto>(stats, "Success"));
        }
    }
}
