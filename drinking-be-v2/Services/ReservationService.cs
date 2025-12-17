using AutoMapper;
using drinking_be.Dtos.ReservationDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReservationReadDto> CreateReservationAsync(ReservationCreateDto dto)
        {
            var reservationRepo = _unitOfWork.Repository<Reservation>();

            // 1. Map DTO -> Entity
            var reservation = _mapper.Map<Reservation>(dto);

            // 2. Sinh mã đặt chỗ (RES-YYYYMMDD-XXXX)
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomPart = new Random().Next(1000, 9999).ToString();
            reservation.ReservationCode = $"RES-{datePart}-{randomPart}";

            // 3. Thiết lập mặc định
            reservation.Status = ReservationStatusEnum.Pending;
            reservation.CreatedAt = DateTime.UtcNow;

            // LOGIC TÍNH TIỀN CỌC: 10.000 VNĐ / người
            decimal depositPerPerson = 10000;
            reservation.DepositAmount = dto.NumberOfGuests * depositPerPerson;
            reservation.IsDepositPaid = false;

            // 4. Lưu vào DB
            await reservationRepo.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để lấy StoreName (nếu cần)
            return (await GetReservationByIdAsync(reservation.Id))!;
        }

        public async Task<ReservationReadDto?> GetReservationByIdAsync(long id)
        {
            var repo = _unitOfWork.Repository<Reservation>();

            // Include Store, User và AssignedTable để hiển thị tên
            var reservation = await repo.GetFirstOrDefaultAsync(
                filter: r => r.Id == id,
                includeProperties: "Store,User,AssignedTable"
            );

            return reservation == null ? null : _mapper.Map<ReservationReadDto>(reservation);
        }

        public async Task<IEnumerable<ReservationReadDto>> GetHistoryByUserIdAsync(int userId)
        {
            var repo = _unitOfWork.Repository<Reservation>();

            var list = await repo.GetAllAsync(
                filter: r => r.UserId == userId,
                orderBy: q => q.OrderByDescending(r => r.ReservationDatetime),
                includeProperties: "Store,AssignedTable"
            );

            return _mapper.Map<IEnumerable<ReservationReadDto>>(list);
        }

        public async Task<IEnumerable<ReservationReadDto>> GetReservationsByStoreAsync(int storeId, DateTime? date, ReservationStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<Reservation>();

            // Query cơ bản theo Store
            var query = await repo.GetAllAsync(
                filter: r => r.StoreId == storeId,
                orderBy: q => q.OrderBy(r => r.ReservationDatetime),
                includeProperties: "User,AssignedTable"
            );

            // Lọc theo ngày (nếu có)
            if (date.HasValue)
            {
                var queryDate = DateOnly.FromDateTime(date.Value);
                query = query.Where(r => DateOnly.FromDateTime(r.ReservationDatetime) == queryDate);
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return _mapper.Map<IEnumerable<ReservationReadDto>>(query);
        }

        public async Task<ReservationReadDto?> UpdateReservationAsync(long id, ReservationUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Reservation>();
            var tableRepo = _unitOfWork.Repository<ShopTable>(); // Cần để check bàn

            var reservation = await repo.GetByIdAsync(id);
            if (reservation == null) return null;

            // 1. Kiểm tra bàn nếu có gán bàn
            if (dto.AssignedTableId.HasValue)
            {
                var table = await tableRepo.GetByIdAsync(dto.AssignedTableId.Value);
                if (table == null || table.StoreId != reservation.StoreId)
                {
                    throw new Exception("Bàn không hợp lệ hoặc không thuộc cửa hàng này.");
                }
                reservation.AssignedTableId = dto.AssignedTableId;
            }

            // 2. Cập nhật các trường khác
            if (dto.Status.HasValue) reservation.Status = dto.Status.Value;
            if (dto.IsDepositPaid.HasValue) reservation.IsDepositPaid = dto.IsDepositPaid.Value;
            if (!string.IsNullOrEmpty(dto.Note)) reservation.Note = dto.Note;

            reservation.UpdatedAt = DateTime.UtcNow;

            repo.Update(reservation);
            await _unitOfWork.SaveChangesAsync();

            return (await GetReservationByIdAsync(id));
        }

        public async Task<bool> CancelReservationAsync(long id, int? userId, string reason)
        {
            var repo = _unitOfWork.Repository<Reservation>();
            var reservation = await repo.GetByIdAsync(id);

            if (reservation == null) return false;

            // 1. Kiểm tra quyền sở hữu (Nếu là User hủy)
            if (userId.HasValue && reservation.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền hủy đơn đặt bàn này.");
            }

            // 2. Ràng buộc trạng thái
            if (reservation.Status == ReservationStatusEnum.Arrived ||
                reservation.Status == ReservationStatusEnum.Completed ||
                reservation.Status == ReservationStatusEnum.Cancelled)
            {
                throw new Exception("Không thể hủy đơn đặt bàn ở trạng thái hiện tại.");
            }

            // 3. Ràng buộc thời gian (Logic 2 giờ)
            double hoursBefore = reservation.ReservationDatetime.Subtract(DateTime.UtcNow).TotalHours;

            if (hoursBefore < 2)
            {
                // Nếu đã đóng cọc và hủy gấp -> Cảnh báo hoặc xử lý mất cọc
                if (reservation.IsDepositPaid)
                {
                    // Logic: Mất cọc (Cập nhật note hoặc status đặc biệt)
                    reservation.Note += " | [HỦY GẤP: Mất cọc]";
                }
            }

            // 4. Cập nhật trạng thái
            reservation.Status = ReservationStatusEnum.Cancelled;
            reservation.Note += string.IsNullOrEmpty(reservation.Note) ? $"[Lý do hủy: {reason}]" : $" | [Lý do hủy: {reason}]";
            reservation.UpdatedAt = DateTime.UtcNow;

            repo.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}