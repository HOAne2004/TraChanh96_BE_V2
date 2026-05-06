using drinking_be.Dtos.GlobalDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

            // 1. Tổng doanh thu (Chỉ tính đơn hoàn thành/đã nhận)
            dto.TotalRevenue = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                .SumAsync(o => (decimal?)o.GrandTotal) ?? 0;

            // 2. Tổng đơn hàng
            dto.TotalOrders = await _unitOfWork.Orders.GetQueryable()
                .CountAsync();

            // 3. Tổng khách hàng hoạt động
            dto.TotalCustomers = await _unitOfWork.Repository<User>().GetQueryable()
                .Where(u => u.RoleId == UserRoleEnum.Customer && u.Status == UserStatusEnum.Active)
                .CountAsync();

            // 4. Tổng sản phẩm đang bán
            dto.TotalProducts = await _unitOfWork.Repository<Product>().GetQueryable()
                .Where(p => p.Status == ProductStatusEnum.Active)
                .CountAsync();

            // 5. Tổng cửa hàng
            dto.TotalStores = await _unitOfWork.Repository<Store>().GetQueryable()
                .Where(s => s.Status == StoreStatusEnum.Active)
                .CountAsync();

            // 6. Đơn hàng gần đây (5 đơn mới nhất)
            var recentOrdersQuery = await _unitOfWork.Orders.GetQueryable()
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new 
                {
                    o.OrderCode,
                    o.RecipientName,
                    o.RecipientPhone,
                    o.CreatedAt,
                    o.GrandTotal,
                    o.Status
                })
                .ToListAsync();

            dto.RecentOrders = recentOrdersQuery.Select(o => new DashboardOrderSummaryDto
            {
                OrderCode = o.OrderCode,
                RecipientName = o.RecipientName,
                RecipientPhone = o.RecipientPhone,
                CreatedAt = o.CreatedAt,
                GrandTotal = o.GrandTotal,
                Status = o.Status.ToString()
            }).ToList();

            // 7. Dữ liệu biểu đồ (7 ngày gần nhất)
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-6);
            
            // Lấy tất cả đơn hàng trong 7 ngày qua để xử lý local cho nhanh và tránh lỗi translation
            var ordersLast7Days = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.CreatedAt >= startDate && 
                       (o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received))
                .Select(o => new { o.CreatedAt, o.GrandTotal })
                .ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                var dayRevenue = ordersLast7Days
                    .Where(o => o.CreatedAt.Date == date)
                    .Sum(o => o.GrandTotal);

                dto.ChartData.Add(new DashboardChartDataDto
                {
                    Label = date.ToString("dd/MM"),
                    Value = dayRevenue
                });
            }

            // 8. Top 5 sản phẩm bán chạy (Sử dụng thông tin Snapshot trên OrderItem để an toàn)
            dto.TopProducts = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.ProductName, oi.ProductImage })
                .Select(g => new DashboardTopProductDto
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.ProductName,
                    Image = g.Key.ProductImage,
                    Sold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => (decimal)oi.Quantity * oi.BasePrice)
                })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync();

            return dto;
        }
    }
}