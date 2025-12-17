using AutoMapper;
using drinking_be.Dtos.AttendanceDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // --- NHÂN VIÊN ---

        public async Task<AttendanceReadDto> CheckInAsync(int staffId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var repo = _unitOfWork.Repository<Attendance>();
            var staffRepo = _unitOfWork.Repository<Staff>();

            // 1. Kiểm tra Staff có tồn tại và đang Active không
            var staff = await staffRepo.GetByIdAsync(staffId);
            if (staff == null || staff.Status != PublicStatusEnum.Active)
            {
                throw new Exception("Nhân viên không tồn tại hoặc đã nghỉ việc.");
            }

            // 2. Kiểm tra hôm nay đã Check-in chưa
            var existing = await repo.GetFirstOrDefaultAsync(a => a.StaffId == staffId && a.Date == today);
            if (existing != null)
            {
                throw new Exception("Hôm nay bạn đã Check-in rồi.");
            }

            // 3. Tạo record Check-in
            int actualStoreId = staff.StoreId ?? 0;

            // Nếu là Admin/HQ (StoreId = null), tạm thời lấy Store đầu tiên trong DB để test
            if (staff.StoreId == null)
            {
                // Cách 1: Gán cứng ID = 1 (Vì trong Seed Data ta đã tạo Store)
                //actualStoreId = 1;

                // Hoặc Cách 2: Báo lỗi (Chuẩn logic thực tế)
                throw new Exception("Nhân viên Văn phòng/HQ không cần chấm công tại Cửa hàng.");
            }

            // 3. Tạo record Check-in
            var attendance = new Attendance
            {
                StaffId = staffId,
                StoreId = actualStoreId, // ✅ Dùng biến đã xử lý
                Date = today,
                CheckInTime = DateTime.UtcNow,
                Status = AttendanceStatusEnum.Present,
                WorkingHours = 0,
                OvertimeHours = 0
            };

            await repo.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            // Load lại để kèm thông tin Staff/Store (nếu cần hiển thị ngay)
            return await GetAttendanceDetailAsync(attendance.Id);
        }

        public async Task<AttendanceReadDto> CheckOutAsync(int staffId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var repo = _unitOfWork.Repository<Attendance>();

            // 1. Tìm record của hôm nay
            var attendance = await repo.GetFirstOrDefaultAsync(a => a.StaffId == staffId && a.Date == today);
            if (attendance == null)
            {
                throw new Exception("Bạn chưa Check-in hôm nay.");
            }

            if (attendance.CheckOutTime != null)
            {
                throw new Exception("Bạn đã Check-out rồi.");
            }

            // 2. Cập nhật giờ ra
            attendance.CheckOutTime = DateTime.UtcNow;

            // 3. Tính giờ làm (Giờ ra - Giờ vào)
            if (attendance.CheckInTime.HasValue)
            {
                var duration = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
                attendance.WorkingHours = Math.Round(duration.TotalHours, 2); // Làm tròn 2 số lẻ

                // Logic tính tăng ca đơn giản (ví dụ: làm hơn 8 tiếng là tăng ca)
                // Bạn có thể tùy chỉnh logic này sau (ví dụ dựa vào Ca làm việc)
                if (attendance.WorkingHours > 8)
                {
                    attendance.OvertimeHours = attendance.WorkingHours - 8;
                    attendance.WorkingHours = 8;
                }
            }

            repo.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            return await GetAttendanceDetailAsync(attendance.Id);
        }

        public async Task<AttendanceReadDto?> GetTodayAttendanceAsync(int staffId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var repo = _unitOfWork.Repository<Attendance>();

            var attendance = await repo.GetFirstOrDefaultAsync(
                a => a.StaffId == staffId && a.Date == today,
                includeProperties: "Staff,Store"
            );

            return attendance == null ? null : _mapper.Map<AttendanceReadDto>(attendance);
        }

        // --- MANAGER ---

        public async Task<IEnumerable<AttendanceReadDto>> GetStaffHistoryAsync(int staffId, int month, int year)
        {
            var repo = _unitOfWork.Repository<Attendance>();

            // Lọc theo Staff và Tháng/Năm
            var list = await repo.GetAllAsync(
                filter: a => a.StaffId == staffId && a.Date.Month == month && a.Date.Year == year,
                orderBy: q => q.OrderByDescending(a => a.Date),
                includeProperties: "Staff,Store"
            );

            return _mapper.Map<IEnumerable<AttendanceReadDto>>(list);
        }

        public async Task<IEnumerable<AttendanceReadDto>> GetStoreDailyReportAsync(int storeId, DateOnly date)
        {
            var repo = _unitOfWork.Repository<Attendance>();

            var list = await repo.GetAllAsync(
                filter: a => a.StoreId == storeId && a.Date == date,
                includeProperties: "Staff,Store"
            );

            return _mapper.Map<IEnumerable<AttendanceReadDto>>(list);
        }

        public async Task<AttendanceReadDto?> UpdateAsync(long id, AttendanceUpdateDto updateDto)
        {
            var repo = _unitOfWork.Repository<Attendance>();
            var attendance = await repo.GetByIdAsync(id);

            if (attendance == null) return null;

            _mapper.Map(updateDto, attendance);

            // Nếu Manager sửa giờ vào/ra, cần tính lại WorkingHours thủ công 
            // hoặc để Manager tự nhập số giờ trong DTO (đơn giản hơn).
            // Ở đây giả định Manager nhập số giờ làm WorkingHours trong DTO luôn.

            attendance.UpdatedAt = DateTime.UtcNow;
            repo.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            return await GetAttendanceDetailAsync(id);
        }

        public async Task<AttendanceReadDto> CreateManualAsync(AttendanceCreateDto createDto)
        {
            var repo = _unitOfWork.Repository<Attendance>();

            // Validate trùng ngày
            var exists = await repo.ExistsAsync(a => a.StaffId == createDto.StaffId && a.Date == createDto.Date);
            if (exists) throw new Exception("Nhân viên này đã có dữ liệu chấm công ngày này rồi.");

            var attendance = _mapper.Map<Attendance>(createDto);

            await repo.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            return await GetAttendanceDetailAsync(attendance.Id);
        }

        // Helper lấy chi tiết để return
        private async Task<AttendanceReadDto> GetAttendanceDetailAsync(long id)
        {
            var att = await _unitOfWork.Repository<Attendance>().GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Staff,Store"
            );
            return _mapper.Map<AttendanceReadDto>(att);
        }
    }
}