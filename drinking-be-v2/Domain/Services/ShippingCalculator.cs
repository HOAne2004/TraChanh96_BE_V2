using drinking_be.Models;
using drinking_be.Utils;

namespace drinking_be.Domain.Services
{
    public class ShippingCalculator
    {
        // Hệ số quy đổi từ đường chim bay sang đường bộ
        private const double ROAD_FACTOR = 1.35;

        // Cấu hình miễn phí vận chuyển
        private const decimal FREE_SHIP_ORDER_THRESHOLD = 500000; // Đơn > 500k
        private const double FREE_SHIP_DISTANCE_KM = 1.0;         // Khoảng cách < 1km

        /// <summary>
        /// Tính phí ship dựa trên Store, Địa chỉ khách và Giá trị đơn hàng
        /// </summary>
        public decimal CalculateFee(Store store, Address customerAddress, decimal orderSubTotal)
        {
            // 1. Rule ưu tiên cao nhất: Đơn hàng giá trị cao (> 500k) -> Free Ship
            if (orderSubTotal > FREE_SHIP_ORDER_THRESHOLD)
            {
                return 0;
            }

            // 2. Validate dữ liệu tọa độ
            if (!IsCoordinatesValid(store.Address) || !IsCoordinatesValid(customerAddress))
            {
                // Fallback: Trả về phí cố định nếu lỗi tọa độ
                return store.ShippingFeeFixed ?? 15000;
            }

            // 3. Tính khoảng cách thực tế ước tính (Km)
            double airlineDistance = DistanceUtils.CalculateDistanceKm(
                store.Address!.Latitude!.Value, store.Address.Longitude!.Value,
                customerAddress.Latitude!.Value, customerAddress.Longitude!.Value
            );

            double roadDistance = airlineDistance * ROAD_FACTOR;

            // 4. Kiểm tra bán kính phục vụ
            double limitKm = store.DeliveryRadius > 0 ? store.DeliveryRadius : 20;
            if (roadDistance > limitKm)
            {
                // Ném lỗi 400 để FE hiển thị thông báo
                throw new Exception($"Khoảng cách giao hàng ước tính ({roadDistance:F1}km) vượt quá phạm vi phục vụ ({limitKm}km) của quán.");
            }

            // 5. Rule ưu tiên nhì: Khoảng cách rất gần (< 1km) -> Free Ship
            if (roadDistance < FREE_SHIP_DISTANCE_KM)
            {
                return 0;
            }

            // 6. Tính phí ship tiêu chuẩn (Standard Logic)
            decimal baseFee = 15000;       // Phí mở cửa (cho 2km đầu)
            double baseDistance = 2.0;     // Khoảng cách mở cửa
            decimal extraFeePerKm = store.ShippingFeePerKm ?? 5000; // Phí mỗi km tiếp theo

            decimal totalFee = 0;

            if (roadDistance <= baseDistance)
            {
                totalFee = baseFee;
            }
            else
            {
                // Phí = 15k + (Số km dư * 5k)
                double extraKm = roadDistance - baseDistance;
                totalFee = baseFee + ((decimal)extraKm * extraFeePerKm);
            }

            // 7. Làm tròn lên hàng nghìn (VD: 16,200 -> 17,000)
            totalFee = Math.Ceiling(totalFee / 1000) * 1000;

            return totalFee;
        }

        private bool IsCoordinatesValid(Address? address)
        {
            return address != null && address.Latitude.HasValue && address.Longitude.HasValue;
        }
    }
}