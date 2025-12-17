using AutoMapper;
using drinking_be.Dtos.PayslipDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class PayslipService : IPayslipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PayslipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PayslipReadDto> GeneratePayslipAsync(PayslipCreateDto createDto)
        {
            var payslipRepo = _unitOfWork.Repository<Payslip>();
            var staffRepo = _unitOfWork.Repository<Staff>();
            var attendanceRepo = _unitOfWork.Repository<Attendance>();

            // 1. Kiểm tra xem phiếu lương tháng này đã tạo chưa
            var exists = await payslipRepo.ExistsAsync(p =>
                p.StaffId == createDto.StaffId &&
                p.Month == createDto.Month &&
                p.Year == createDto.Year);

            if (exists) throw new Exception($"Phiếu lương tháng {createDto.Month}/{createDto.Year} của nhân viên này đã tồn tại.");

            // 2. Lấy thông tin nhân viên
            var staff = await staffRepo.GetByIdAsync(createDto.StaffId);
            if (staff == null) throw new Exception("Nhân viên không tồn tại.");

            // 3. Xác định khoảng thời gian (Từ ngày 1 đến cuối tháng)
            var fromDate = createDto.FromDate ?? new DateOnly(createDto.Year, createDto.Month, 1);
            var toDate = createDto.ToDate ?? fromDate.AddMonths(1).AddDays(-1);

            // 4. Lấy dữ liệu chấm công trong khoảng thời gian đó
            var attendances = await attendanceRepo.GetAllAsync(
                filter: a => a.StaffId == createDto.StaffId && a.Date >= fromDate && a.Date <= toDate
            );

            // 5. Khởi tạo Payslip với thông tin Snapshot (Lưu lại mức lương tại thời điểm tính)
            var payslip = new Payslip
            {
                StaffId = createDto.StaffId,
                Month = createDto.Month,
                Year = createDto.Year,
                FromDate = fromDate,
                ToDate = toDate,
                Status = PayslipStatusEnum.Draft, // Mới tạo là Nháp

                // Snapshot cấu hình lương từ Staff
                AppliedSalaryType = staff.SalaryType,
                AppliedBaseSalary = staff.BaseSalary ?? 0,
                AppliedHourlyRate = staff.HourlySalary ?? 0,
                AppliedOvertimeRate = staff.OvertimeHourlySalary ?? 0,
            };

            // 6. TÍNH TOÁN (Aggregation)
            // Tổng hợp từ Attendance
            payslip.TotalWorkDays = attendances.Count();
            payslip.TotalWorkHours = attendances.Sum(a => a.WorkingHours);
            payslip.TotalOvertimeHours = attendances.Sum(a => a.OvertimeHours);

            // Tổng thưởng/phạt hàng ngày (Daily Bonus/Deduction)
            var totalDailyBonus = attendances.Sum(a => a.DailyBonus);
            var totalDailyDeduction = attendances.Sum(a => a.DailyDeduction);

            // Tính Lương Cơ bản (SalaryBeforeTax)
            if (staff.SalaryType == SalaryTypeEnum.FullTime)
            {
                // Fulltime: Lương cứng + (Giờ OT * Rate OT)
                // Lưu ý: Có thể chia lương cứng theo ngày công thực tế nếu muốn (Logic phức tạp hơn)
                // Ở đây ta tính đơn giản: Full lương cứng nếu đi làm đủ (hoặc Manager tự trừ sau)
                payslip.SalaryBeforeTax = payslip.AppliedBaseSalary
                                          + (decimal)payslip.TotalOvertimeHours * payslip.AppliedOvertimeRate;
            }
            else
            {
                // Parttime: (Giờ làm * Rate) + (Giờ OT * Rate OT)
                payslip.SalaryBeforeTax = ((decimal)payslip.TotalWorkHours * payslip.AppliedHourlyRate)
                                          + ((decimal)payslip.TotalOvertimeHours * payslip.AppliedOvertimeRate);
            }

            // Cộng dồn thưởng/phạt
            payslip.Bonus = totalDailyBonus;
            payslip.Deduction = totalDailyDeduction;
            payslip.Allowance = 0; // Mặc định 0, Manager update sau nếu có phụ cấp riêng

            // 7. Tính Thực Lĩnh (FinalSalary)
            CalculateFinalSalary(payslip);

            // 8. Lưu vào DB
            await payslipRepo.AddAsync(payslip);
            await _unitOfWork.SaveChangesAsync();

            // Load lại kèm thông tin Staff để trả về DTO đẹp
            return await GetPayslipDetailAsync(payslip.Id);
        }

        public async Task<IEnumerable<PayslipReadDto>> GetAllAsync(int? storeId, int? month, int? year)
        {
            var repo = _unitOfWork.Repository<Payslip>();

            var query = await repo.GetAllAsync(
                includeProperties: "Staff"
            );

            // Lọc dữ liệu
            if (storeId.HasValue) query = query.Where(p => p.Staff.StoreId == storeId);
            if (month.HasValue) query = query.Where(p => p.Month == month);
            if (year.HasValue) query = query.Where(p => p.Year == year);

            return _mapper.Map<IEnumerable<PayslipReadDto>>(query);
        }

        public async Task<PayslipReadDto?> GetByIdAsync(long id)
        {
            return await GetPayslipDetailAsync(id);
        }

        public async Task<PayslipReadDto?> UpdateAsync(long id, PayslipUpdateDto updateDto)
        {
            var repo = _unitOfWork.Repository<Payslip>();
            var payslip = await repo.GetByIdAsync(id);

            if (payslip == null) return null;

            // Chỉ cho phép sửa khi chưa thanh toán (Paid)
            if (payslip.Status == PayslipStatusEnum.Paid)
            {
                throw new Exception("Không thể chỉnh sửa phiếu lương đã thanh toán.");
            }

            // Map dữ liệu update (Bonus, Deduction, Note, Status...)
            _mapper.Map(updateDto, payslip);

            // Tính toán lại thực lĩnh sau khi sửa số liệu
            CalculateFinalSalary(payslip);

            payslip.UpdatedAt = DateTime.UtcNow;
            repo.Update(payslip);
            await _unitOfWork.SaveChangesAsync();

            return await GetPayslipDetailAsync(id);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var repo = _unitOfWork.Repository<Payslip>();
            var payslip = await repo.GetByIdAsync(id);

            if (payslip == null) return false;

            // Chỉ xóa được nháp
            if (payslip.Status != PayslipStatusEnum.Draft)
            {
                throw new Exception("Chỉ có thể xóa phiếu lương ở trạng thái Nháp.");
            }

            repo.Delete(payslip);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // --- Helper Methods ---

        private void CalculateFinalSalary(Payslip p)
        {
            // Công thức: Lương thô + Phụ cấp + Thưởng - Phạt - Thuế
            p.FinalSalary = p.SalaryBeforeTax + p.Allowance + p.Bonus - p.Deduction - p.TaxAmount;
        }

        private async Task<PayslipReadDto> GetPayslipDetailAsync(long id)
        {
            var p = await _unitOfWork.Repository<Payslip>().GetFirstOrDefaultAsync(
                filter: x => x.Id == id,
                includeProperties: "Staff"
            );
            return _mapper.Map<PayslipReadDto>(p);
        }
    }
}