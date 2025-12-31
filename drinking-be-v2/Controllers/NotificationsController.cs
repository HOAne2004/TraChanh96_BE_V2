using drinking_be.Dtos.NotificationDtos;
using drinking_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notiService;

        public NotificationsController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        private int GetUserId()
        {
            return User.GetUserId();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _notiService.GetMyNotificationsAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Chỉ Admin được gửi thông báo thủ công
        public async Task<IActionResult> Create([FromBody] NotificationCreateDto dto)
        {
            var result = await _notiService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            await _notiService.MarkAsReadAsync(id, GetUserId());
            return NoContent();
        }

        [HttpGet("manage")]
        [Authorize(Roles = "Admin,Manager")] // Chỉ cho phép quản lý
        public async Task<IActionResult> GetListForAdmin([FromQuery] NotificationFilterDto filter)
        {
            var result = await _notiService.GetNotificationsByFilterAsync(filter);
            return Ok(result);
        }
    }
}