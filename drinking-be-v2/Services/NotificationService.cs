using AutoMapper;
using drinking_be.Dtos.NotificationDtos;
using drinking_be.Interfaces;
using drinking_be.Models;

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
    }

    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            return _mapper.Map<NotificationReadDto>(noti);
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
    }
}