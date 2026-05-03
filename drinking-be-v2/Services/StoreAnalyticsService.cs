using drinking_be.Dtos.StoreDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class StoreAnalyticsService : IStoreAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreDashboardDto> GetStoreDashboardAsync(int storeId, string timeFilter)
        {
            // 1. Xác định mốc thời gian (Hiện tại & Kỳ trước để tính Growth)
            var (startDate, endDate, prevStartDate, prevEndDate) = GetDateRanges(timeFilter);

            var dto = new StoreDashboardDto();

            // 2. LẤY DỮ LIỆU ĐƠN HÀNG (Chỉ lấy đơn đã thanh toán hoặc hoàn thành)
            var ordersQuery = _unitOfWork.Orders.GetQueryable()
                .Where(o => o.StoreId == storeId && o.Status != OrderStatusEnum.Cancelled && o.Status != OrderStatusEnum.Deleted);

            // Dữ liệu kỳ này
            var currentOrders = await ordersQuery
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Select(o => new { o.Id, o.CreatedAt, o.GrandTotal, o.UserId })
                .ToListAsync();

            // Dữ liệu kỳ trước
            var prevOrders = await ordersQuery
                .Where(o => o.CreatedAt >= prevStartDate && o.CreatedAt <= prevEndDate)
                .Select(o => new { o.Id, o.GrandTotal, o.UserId })
                .ToListAsync();

            // --- TÍNH TOÁN STATS CHÍNH ---
            dto.Revenue = currentOrders.Sum(o => o.GrandTotal);
            var prevRevenue = prevOrders.Sum(o => o.GrandTotal);
            dto.RevenueGrowth = CalculateGrowth(dto.Revenue, prevRevenue);

            dto.OrdersCount = currentOrders.Count;
            dto.OrdersGrowth = CalculateGrowth(dto.OrdersCount, prevOrders.Count);

            dto.CustomersCount = currentOrders.Where(o => o.UserId.HasValue).Select(o => o.UserId).Distinct().Count();
            var prevCustomersCount = prevOrders.Where(o => o.UserId.HasValue).Select(o => o.UserId).Distinct().Count();
            dto.CustomersGrowth = CalculateGrowth(dto.CustomersCount, prevCustomersCount);

            // 3. TÌNH TRẠNG BÀN (Real-time)
            var tables = await _unitOfWork.Repository<ShopTable>().GetAllAsync(t => t.StoreId == storeId);
            dto.TablesTotal = tables.Count();
            dto.TablesInUse = tables.Count(t => t.Status == TableStatusEnum.Occupied); 

            // 4. TOP 5 SẢN PHẨM BÁN CHẠY (Join OrderItems và Products)
            var topProductsQuery = await _unitOfWork.Repository<OrderItem>().GetQueryable()
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.StoreId == storeId
                          && oi.Order.CreatedAt >= startDate
                          && oi.Order.CreatedAt <= endDate
                          && oi.Order.Status != OrderStatusEnum.Cancelled)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.ImageUrl })
                .Select(g => new TopProductDto
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.Name,
                    Image = g.Key.ImageUrl,
                    Sold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.FinalPrice * x.Quantity) // Doanh thu từ món này
                })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync();

            dto.TopProducts = topProductsQuery;

            // 5. ĐÁNH GIÁ (REVIEWS)
            var reviewsQuery = _unitOfWork.Repository<Review>().GetQueryable()
                .Where(r => r.Order.StoreId == storeId);

            var currentReviews = await reviewsQuery.Where(r => r.CreatedAt <= endDate).ToListAsync();
            var prevReviews = await reviewsQuery.Where(r => r.CreatedAt <= prevEndDate).ToListAsync();

            dto.RatingCount = currentReviews.Count;
            dto.RatingAverage = currentReviews.Any() ? Math.Round(currentReviews.Average(r => r.Rating), 1) : 0;

            var prevRatingAvg = prevReviews.Any() ? prevReviews.Average(r => r.Rating) : 0;
            dto.RatingGrowth = Math.Round(dto.RatingAverage - prevRatingAvg, 1); // Tăng/Giảm bao nhiêu điểm

            // 6. DỮ LIỆU BIỂU ĐỒ (CHART DATA)
            dto.ChartData = GenerateChartData(currentOrders.Select(o => (o.CreatedAt, o.GrandTotal)), startDate, endDate, timeFilter);

            return dto;
        }

        // --- CÁC HÀM HELPER BÊN DƯỚI ---

        private double CalculateGrowth(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return (double)Math.Round(((current - previous) / previous) * 100, 1);
        }

        private double CalculateGrowth(int current, int previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return Math.Round(((double)(current - previous) / previous) * 100, 1);
        }

        private (DateTime Start, DateTime End, DateTime PrevStart, DateTime PrevEnd) GetDateRanges(string filter)
        {
            var now = DateTime.UtcNow; // Nhớ dùng UTC hoặc Local tùy DB của bạn
            DateTime start, end, prevStart, prevEnd;

            switch (filter.ToLower())
            {
                case "today":
                    start = now.Date;
                    end = start.AddDays(1).AddTicks(-1);
                    prevStart = start.AddDays(-1);
                    prevEnd = prevStart.AddDays(1).AddTicks(-1);
                    break;
                case "week":
                    int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = now.AddDays(-1 * diff).Date;
                    end = start.AddDays(7).AddTicks(-1);
                    prevStart = start.AddDays(-7);
                    prevEnd = prevStart.AddDays(7).AddTicks(-1);
                    break;
                case "month":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddTicks(-1);
                    prevStart = start.AddMonths(-1);
                    prevEnd = prevStart.AddMonths(1).AddTicks(-1);
                    break;
                case "year":
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1).AddTicks(-1);
                    prevStart = start.AddYears(-1);
                    prevEnd = prevStart.AddYears(1).AddTicks(-1);
                    break;
                default: goto case "today";
            }
            return (start, end, prevStart, prevEnd);
        }

        private List<ChartDataDto> GenerateChartData(IEnumerable<(DateTime Date, decimal Amount)> data, DateTime start, DateTime end, string filter)
        {
            var chart = new List<ChartDataDto>();

            if (filter == "today")
            {
                // Group theo giờ
                for (int i = 8; i <= 22; i += 2) // Quán mở từ 8h đến 22h
                {
                    var sum = data.Where(d => d.Date.Hour >= i && d.Date.Hour < i + 2).Sum(d => d.Amount);
                    chart.Add(new ChartDataDto { Label = $"{i}:00", Value = sum });
                }
            }
            else if (filter == "week" || filter == "month")
            {
                // Group theo ngày
                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    var sum = data.Where(d => d.Date.Date == date.Date).Sum(d => d.Amount);
                    chart.Add(new ChartDataDto { Label = date.ToString("dd/MM"), Value = sum });
                }
            }
            else if (filter == "year")
            {
                // Group theo tháng
                for (int i = 1; i <= 12; i++)
                {
                    var sum = data.Where(d => d.Date.Month == i).Sum(d => d.Amount);
                    chart.Add(new ChartDataDto { Label = $"Tháng {i}", Value = sum });
                }
            }

            return chart;
        }
    }
}