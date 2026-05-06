using System.Collections.Generic;

namespace drinking_be.Dtos.GlobalDtos
{
    public class GlobalDashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalStores { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal RevenueGrowth { get; set; }
        public int OrderGrowth { get; set; }
        public int NewCustomersToday { get; set; }

        // Danh sách bổ sung cho Dashboard
        public List<DashboardOrderSummaryDto> RecentOrders { get; set; } = new();
        public List<DashboardChartDataDto> ChartData { get; set; } = new();
        public List<DashboardTopProductDto> TopProducts { get; set; } = new();
    }

    public class DashboardOrderSummaryDto
    {
        public string OrderCode { get; set; } = null!;
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = null!;
    }

    public class DashboardChartDataDto
    {
        public string Label { get; set; } = null!;
        public decimal Value { get; set; }
    }

    public class DashboardTopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
    }
}
