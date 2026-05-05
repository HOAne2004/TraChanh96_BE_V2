using drinking_be.Dtos.GlobalDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public interface ISystemAnalyticsService
    {
        Task<GlobalDashboardStatsDto> GetGlobalStatsAsync();
    }
    public class SystemAnalyticsService : ISystemAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GlobalDashboardStatsDto> GetGlobalStatsAsync()
        {
            var dto = new GlobalDashboardStatsDto();

            // 1. Tổng tiền (Chỉ tính đơn hoàn thành)
            // Lưu ý: Cần tối ưu bằng cách lưu Cache nếu bảng Order quá lớn
            dto.TotalRevenue = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                .SumAsync(o => (decimal?)o.GrandTotal) ?? 0;

            // 2. Tổng cửa hàng (Chỉ đếm cửa hàng Active)
            dto.TotalStores = await _unitOfWork.Repository<Store>().GetQueryable()
                .Where(s => s.Status == StoreStatusEnum.Active)
                .CountAsync();

            // 3. Tổng sản phẩm (Chỉ đếm sản phẩm đang kinh doanh)
            dto.TotalProducts = await _unitOfWork.Repository<Product>().GetQueryable()
                .Where(p => p.Status == ProductStatusEnum.Active)
                .CountAsync();

            // 4. Tổng số khách hàng đã đăng ký (Đếm User có Role là Customer)
            dto.TotalCustomers = await _unitOfWork.Repository<User>().GetQueryable()
                .Where(u => u.RoleId == UserRoleEnum.Customer && u.Status == UserStatusEnum.Active)
                .CountAsync();

            return dto;
        }
    }
}