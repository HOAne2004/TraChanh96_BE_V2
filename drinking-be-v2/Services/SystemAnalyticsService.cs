using drinking_be.Dtos.GlobalDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public interface ISystemAnalyticsService
    {
        Task<GlobalDashboardStatsDto> GetGlobalStatsAsync(string timeframe);
    }

    public class SystemAnalyticsService : ISystemAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GlobalDashboardStatsDto> GetGlobalStatsAsync(string timeframe)
        {
            var dto = new GlobalDashboardStatsDto();

            // Dùng giờ Việt Nam (UTC+7) để thống kê chính xác thay vì giờ UTC gốc của Server
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            DateTime? startDate = null;

            // 1. Lọc "chuẩn" theo thời gian thực (Tuần này, Tháng này, Năm nay)
            switch (timeframe?.ToLower())
            {
                case "today":
                    startDate = now.Date;
                    break;
                case "week":
                    // Lấy ngày Thứ 2 của tuần hiện tại
                    int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = now.Date.AddDays(-1 * diff);
                    break;
                case "month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
                case "year":
                    startDate = new DateTime(now.Year, 1, 1);
                    break;
                case "all":
                default:
                    startDate = null; // Lấy từ thuở khai thiên lập địa
                    break;
            }

            var ordersQuery = _unitOfWork.Orders.GetQueryable();
            var completedOrdersQuery = ordersQuery.Where(o =>
                o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received);

            if (startDate.HasValue)
            {
                // Chuyển startDate về lại UTC để so sánh với Database
                var utcStartDate = TimeZoneInfo.ConvertTimeToUtc(startDate.Value, vnTimeZone);
                ordersQuery = ordersQuery.Where(o => o.CreatedAt >= utcStartDate);
                completedOrdersQuery = completedOrdersQuery.Where(o => o.CreatedAt >= utcStartDate);
            }

            // 2. Các thông số tổng
            dto.TotalRevenue = await completedOrdersQuery.SumAsync(o => (decimal?)o.GrandTotal) ?? 0;
            dto.TotalOrders = await ordersQuery.CountAsync();

            // Khách hàng & Sản phẩm thường xem tổng số hiện có
            dto.TotalCustomers = await _unitOfWork.Repository<User>().GetQueryable()
                .CountAsync(u => u.RoleId == UserRoleEnum.Customer && u.Status == UserStatusEnum.Active);
            dto.TotalProducts = await _unitOfWork.Repository<Product>().GetQueryable()
                .CountAsync(p => p.Status == ProductStatusEnum.Active);

            // 3. Đơn hàng gần đây
            dto.RecentOrders = await ordersQuery
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new DashboardOrderSummaryDto
                {
                    OrderCode = o.OrderCode ?? "N/A",
                    RecipientName = o.RecipientName,
                    RecipientPhone = o.RecipientPhone,
                    CreatedAt = o.CreatedAt,
                    GrandTotal = o.GrandTotal,
                    Status = o.Status.ToString()
                })
                .ToListAsync();

            // 4. Biểu đồ (Logic động)
            await BuildChartData(dto, timeframe, completedOrdersQuery, now);

            // 5. Top 5 sản phẩm
            dto.TopProducts = await completedOrdersQuery
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.ProductName, oi.ProductImage })
                .Select(g => new DashboardTopProductDto
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.ProductName ?? "Không xác định",
                    Image = g.Key.ProductImage,
                    Sold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => (decimal)oi.Quantity * oi.BasePrice)
                })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync();

            return dto;
        }

        // HÀM VẼ BIỂU ĐỒ ĐỘNG
        private async Task BuildChartData(GlobalDashboardStatsDto dto, string timeframe, IQueryable<Order> query, DateTime now)
        {
            if (timeframe == "year")
            {
                // VẼ 12 THÁNG
                var rawData = await query.Select(o => new { o.CreatedAt.Month, o.GrandTotal }).ToListAsync();
                for (int i = 1; i <= 12; i++)
                {
                    dto.ChartData.Add(new DashboardChartDataDto
                    {
                        Label = $"Tháng {i}",
                        Value = rawData.Where(x => x.Month == i).Sum(x => x.GrandTotal)
                    });
                }
            }
            else if (timeframe == "all")
            {
                // VẼ CÁC NĂM
                var rawData = await query.Select(o => new { o.CreatedAt.Year, o.GrandTotal }).ToListAsync();
                var minYear = rawData.Any() ? rawData.Min(x => x.Year) : now.Year;

                for (int i = minYear; i <= now.Year; i++)
                {
                    dto.ChartData.Add(new DashboardChartDataDto
                    {
                        Label = $"Năm {i}",
                        Value = rawData.Where(x => x.Year == i).Sum(x => x.GrandTotal)
                    });
                }
            }
            else if (timeframe == "month")
            {
                // VẼ TỪNG NGÀY TRONG THÁNG NÀY (vd: 1->31)
                var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                var rawData = await query.Select(o => new { o.CreatedAt.Day, o.GrandTotal }).ToListAsync();

                for (int i = 1; i <= daysInMonth; i++)
                {
                    dto.ChartData.Add(new DashboardChartDataDto
                    {
                        Label = $"{i:00}/{now.Month:00}",
                        Value = rawData.Where(x => x.Day == i).Sum(x => x.GrandTotal)
                    });
                }
            }
            else if (timeframe == "week")
            {
                // VẼ 7 NGÀY TRONG TUẦN (Thứ 2 -> Chủ Nhật)
                var rawData = await query.Select(o => new { o.CreatedAt.Date, o.GrandTotal }).ToListAsync();

                int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                var startOfWeek = now.Date.AddDays(-1 * diff);
                string[] dayNames = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };

                for (int i = 0; i < 7; i++)
                {
                    var currentDay = startOfWeek.AddDays(i);
                    dto.ChartData.Add(new DashboardChartDataDto
                    {
                        Label = $"{dayNames[i]} ({currentDay:dd/MM})",
                        Value = rawData.Where(x => x.Date == currentDay).Sum(x => x.GrandTotal)
                    });
                }
            }
            else // today
            {
                // VẼ THEO GIỜ TRONG NGÀY (Gộp mỗi 2 tiếng 1 cột cho đẹp)
                var rawData = await query.Select(o => new { o.CreatedAt.Hour, o.GrandTotal }).ToListAsync();
                for (int i = 0; i < 24; i += 2)
                {
                    dto.ChartData.Add(new DashboardChartDataDto
                    {
                        Label = $"{i:00}h",
                        Value = rawData.Where(x => x.Hour == i || x.Hour == i + 1).Sum(x => x.GrandTotal)
                    });
                }
            }
        }
    }
}