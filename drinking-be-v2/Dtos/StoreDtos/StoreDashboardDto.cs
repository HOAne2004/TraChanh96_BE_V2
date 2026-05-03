namespace drinking_be.Dtos.StoreDtos
{
    public class StoreDashboardDto
    {
        // Thống kê chính
        public decimal Revenue { get; set; }
        public double RevenueGrowth { get; set; } // % tăng trưởng

        public int TablesInUse { get; set; }
        public int TablesTotal { get; set; }

        public int OrdersCount { get; set; }
        public double OrdersGrowth { get; set; } // % tăng trưởng

        public int CustomersCount { get; set; }
        public double CustomersGrowth { get; set; } // % tăng trưởng

        // Đánh giá
        public double RatingAverage { get; set; }
        public int RatingCount { get; set; }
        public double RatingGrowth { get; set; } // Điểm tăng/giảm (VD: +0.2)

        // Top sản phẩm
        public List<TopProductDto> TopProducts { get; set; } = new();

        // Biểu đồ
        public List<ChartDataDto> ChartData { get; set; } = new();
    }

    public class TopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ChartDataDto
    {
        public string Label { get; set; } = null!;
        public decimal Value { get; set; }
    }
}