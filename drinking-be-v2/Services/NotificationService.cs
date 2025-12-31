using AutoMapper;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.NotificationDtos;
using drinking_be.Hubs;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public interface INotificationService
    {
        // Lấy thông báo của User (Bao gồm cả thông báo riêng + thông báo chung)
        Task<IEnumerable<NotificationReadDto>> GetMyNotificationsAsync(int userId);
        // Tạo thông báo (Admin/System gọi)
        Task<NotificationReadDto> CreateAsync(NotificationCreateDto dto);
        // Đánh dấu đã đọc
        Task MarkAsReadAsync(long id, int userId);

        Task<PagedResult<NotificationReadDto>> GetNotificationsByFilterAsync(NotificationFilterDto filter);
    }

    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationReadDto>> GetMyNotificationsAsync(int userId)
        {
            var repo = _unitOfWork.Repository<Notification>();
            var now = DateTime.UtcNow;

            // Logic lấy thông báo:
            // 1. Của User đó HOẶC của tất cả (UserId == null)
            // 2. VÀ (ScheduledTime là null HOẶC Đã đến giờ gửi)
            var notis = await repo.GetAllAsync(
                filter: n => (n.UserId == userId || n.UserId == null)
                             && (n.ScheduledTime == null || n.ScheduledTime <= now),
                orderBy: q => q.OrderByDescending(n => n.ScheduledTime ?? n.CreatedAt)
            // Ưu tiên sắp xếp theo thời gian gửi, nếu không có thì lấy ngày tạo
            );

            return _mapper.Map<IEnumerable<NotificationReadDto>>(notis);
        }

        public async Task<NotificationReadDto> CreateAsync(NotificationCreateDto dto)
        {
            var noti = _mapper.Map<Notification>(dto);
            noti.CreatedAt = DateTime.UtcNow;
            noti.IsRead = false;

            await _unitOfWork.Repository<Notification>().AddAsync(noti);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<NotificationReadDto>(noti);

            // 🟢 REAL-TIME: Bắn thông báo qua SignalR
            if (dto.UserId.HasValue)
            {
                // Gửi riêng cho User cụ thể
                // Lưu ý: UserId trong Claims phải match với format chuỗi của UserIdentifier
                await _hubContext.Clients.User(dto.UserId.Value.ToString())
                    .SendAsync("ReceiveNotification", resultDto);
            }
            else
            {
                // Gửi cho tất cả (Broadcast)
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", resultDto);
            }

            return resultDto;
        }

        public async Task MarkAsReadAsync(long id, int userId)
        {
            var repo = _unitOfWork.Repository<Notification>();
            var noti = await repo.GetFirstOrDefaultAsync(n => n.Id == id);

            if (noti != null)
            {
                // Chỉ cho phép đánh dấu nếu là thông báo riêng của user
                // (Với thông báo chung, logic IsRead phức tạp hơn cần bảng trung gian, 
                // ở mức đơn giản này ta chỉ xử lý IsRead cho thông báo cá nhân)
                if (noti.UserId == userId)
                {
                    noti.IsRead = true;
                    repo.Update(noti);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
        public async Task<PagedResult<NotificationReadDto>> GetNotificationsByFilterAsync(NotificationFilterDto filter)
        {
            var query = _unitOfWork.Repository<Notification>().GetQueryable();

            // 1. Lọc theo từ khóa
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(n => n.Title.Contains(filter.Keyword) || n.Content.Contains(filter.Keyword));
            }

            // 2. Lọc theo loại
            if (filter.Type.HasValue)
            {
                query = query.Where(n => n.Type == filter.Type.Value);
            }

            // 3. Đếm tổng
            int totalRow = await query.CountAsync();

            // 4. Phân trang & Sort (Mới nhất lên đầu)
            var data = await query.OrderByDescending(n => n.CreatedAt)
                                  .Skip((filter.PageIndex - 1) * filter.PageSize)
                                  .Take(filter.PageSize)
                                  .ToListAsync();

            var dtos = _mapper.Map<List<NotificationReadDto>>(data);

            return new PagedResult<NotificationReadDto>(dtos, totalRow, filter.PageIndex, filter.PageSize);
        }
    }
}