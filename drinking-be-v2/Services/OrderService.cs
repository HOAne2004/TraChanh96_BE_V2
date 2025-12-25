using AutoMapper;
using drinking_be.Data; // Chứa IUnitOfWork
using drinking_be.Dtos.Common;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Dtos.OrderItemDtos;
using drinking_be.Enums;
using drinking_be.Interfaces; // Chứa IUnitOfork
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // =========================================================================
        // 1. TẠO ĐƠN GIAO HÀNG (DELIVERY)
        // =========================================================================
        public async Task<OrderReadDto> CreateDeliveryOrderAsync(int? userId, DeliveryOrderCreateDto dto)
        {
            // 1. Validate Store
            var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId);
            if (store == null) throw new KeyNotFoundException("Cửa hàng không tồn tại.");
            if (store.Status != StoreStatusEnum.Active) throw new Exception("Cửa hàng hiện đang đóng cửa.");

            // 2. Validate Address
            var address = await _unitOfWork.Addresses.GetByIdAsync(dto.DeliveryAddressId);
            if (address == null) throw new KeyNotFoundException("Địa chỉ giao hàng không tồn tại.");

            // 3. Khởi tạo Order Entity (Map cơ bản từ DTO)
            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            order.OrderCode = GenerateOrderCode(); // Hàm tự sinh mã: ORDER_TIMESTAMP
            order.Status = OrderStatusEnum.New;

            // 4. Xử lý danh sách món & Tính tiền hàng (SubTotal)
            var (orderItems, totalItemAmount) = await ProcessOrderItemsAsync(dto.Items, dto.StoreId);
            order.OrderItems = orderItems;
            order.TotalAmount = totalItemAmount;

            // 5. Tính phí Ship (Dùng Haversine)
            order.ShippingFee = CalculateShippingFee(store, address);

            // 6. Tính tổng tiền & Voucher (Tạm thời chưa trừ Voucher phức tạp)
            order.DiscountAmount = 0; // TODO: Gọi VoucherService để tính
            order.GrandTotal = order.TotalAmount + (order.ShippingFee ?? 0) - (order.DiscountAmount ?? 0);

            // 7. Tính điểm thưởng (Ví dụ: 1% giá trị đơn hàng)
            order.CoinsEarned = (int)(order.GrandTotal * 0.01m);

            // 8. Lưu xuống DB
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            // 9. Return DTO (Query lại để lấy đầy đủ thông tin Include như Product Name, Store Name)
            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // 2. TẠO ĐƠN TẠI QUẦY (AT COUNTER)
        // =========================================================================
        public async Task<OrderReadDto> CreateAtCounterOrderAsync(int? userId, AtCounterOrderCreateDto dto)
        {
            // 1. Validate Store
            var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId);
            if (store == null) throw new KeyNotFoundException("Cửa hàng không tồn tại.");

            // 2. Validate Table (Nếu có chọn bàn)
            if (dto.TableId.HasValue)
            {
                var table = await _unitOfWork.ShopTables.GetByIdAsync(dto.TableId.Value);
                if (table == null || table.StoreId != dto.StoreId)
                    throw new Exception("Bàn không hợp lệ hoặc không thuộc cửa hàng này.");
            }

            // 3. Khởi tạo Order
            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            order.OrderCode = GenerateOrderCode();
            order.Status = OrderStatusEnum.New;

            // 4. Xử lý món
            var (orderItems, totalItemAmount) = await ProcessOrderItemsAsync(dto.Items, dto.StoreId);
            order.OrderItems = orderItems;
            order.TotalAmount = totalItemAmount;

            // 5. Tại quầy không có phí Ship
            order.ShippingFee = 0;
            order.DeliveryAddressId = null;

            // 6. Tính tổng
            order.DiscountAmount = 0;
            order.GrandTotal = order.TotalAmount - (order.DiscountAmount ?? 0);
            order.CoinsEarned = (int)(order.GrandTotal * 0.01m);

            // 7. Lưu DB
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // 3. PRIVATE: XỬ LÝ MÓN ĂN & TOPPING (LOGIC CỐT LÕI)
        // =========================================================================
        private async Task<(List<OrderItem> Items, decimal TotalAmount)> ProcessOrderItemsAsync(List<OrderItemCreateDto> itemsDto, int storeId)
        {
            var resultItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in itemsDto)
            {
                // A. Lấy thông tin sản phẩm
                // Lưu ý: Đúng ra phải check ProductStore để xem quán này có bán món này không
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product == null) throw new Exception($"Sản phẩm ID {itemDto.ProductId} không tồn tại.");
                if (product.Status != ProductStatusEnum.Active) throw new Exception($"Sản phẩm {product.Name} đang ngừng kinh doanh.");

                // B. Tính giá cơ bản (Có thể lấy từ ProductStore nếu có cấu hình giá riêng)
                decimal unitPrice = product.BasePrice;

                // C. Xử lý Size (Nếu có)
                //Size? size = null;
                if (itemDto.SizeId.HasValue)
                {
                    // Lấy thông tin size & giá chênh lệch từ bảng ProductSize
                    // Giả sử UnitOfWork có GenericRepo cho ProductSize hoặc query trực tiếp
                    // Ở đây query nhanh để demo logic:
                    var productSize = await _unitOfWork.ProductSizes.Find(ps => ps.ProductId == itemDto.ProductId && ps.SizeId == itemDto.SizeId)
                                        .FirstOrDefaultAsync();

                    if (productSize != null)
                    {
                        unitPrice += productSize.PriceOverride ?? 0; // Hoặc cộng thêm PriceModifier của bảng Size
                    }
                }

                // D. Tạo Entity Món Chính
                var mainItem = _mapper.Map<OrderItem>(itemDto);
                mainItem.BasePrice = product.BasePrice;
                mainItem.FinalPrice = unitPrice; // Giá chưa nhân số lượng, chưa cộng topping

                decimal itemTotal = unitPrice;

                // E. Xử lý Topping (Đệ quy 1 cấp)
                if (itemDto.Toppings != null && itemDto.Toppings.Any())
                {
                    foreach (var toppingDto in itemDto.Toppings)
                    {
                        var toppingProduct = await _unitOfWork.Products.GetByIdAsync(toppingDto.ProductId);
                        if (toppingProduct == null) continue;

                        var toppingItem = _mapper.Map<OrderItem>(toppingDto);
                        toppingItem.BasePrice = toppingProduct.BasePrice;
                        toppingItem.FinalPrice = toppingProduct.BasePrice; // Topping thường không có size

                        // Link topping vào món chính
                        mainItem.InverseParentItem.Add(toppingItem);

                        // Cộng tiền topping vào tổng giá món chính (để tính GrandTotal)
                        // Lưu ý: Logic này tùy thuộc vào việc bạn muốn lưu giá topping riêng hay gộp.
                        // Ở đây ta cộng dồn vào tổng đơn hàng:
                        totalAmount += (toppingItem.FinalPrice * toppingItem.Quantity);
                    }
                }

                // F. Cộng tiền món chính vào tổng đơn
                totalAmount += (mainItem.FinalPrice * mainItem.Quantity);

                resultItems.Add(mainItem);
            }

            return (resultItems, totalAmount);
        }

        // =========================================================================
        // 4. PRIVATE: TÍNH PHÍ SHIP (HAVERSINE)
        // =========================================================================
        private decimal CalculateShippingFee(Store store, Address address)
        {
            if (!address.Latitude.HasValue || !address.Longitude.HasValue)
                return store.ShippingFeeFixed ?? 15000; // Mặc định nếu không có tọa độ

            // Giả sử Store chưa có tọa độ trong DB thì lấy tọa độ gốc của hệ thống hoặc trả về phí cứng
            // Ở đây giả định Store.Address (Include) có tọa độ.
            // Nếu Store entity chưa include Address, cần cẩn thận null ref.
            // Tạm tính khoảng cách = 0 nếu thiếu data store.
            double storeLat = 10.7769; // Ví dụ tọa độ HCM (Cần lấy từ store.Address)
            double storeLon = 106.7009;

            double distanceKm = DistanceUtils.CalculateDistanceKm(
                storeLat, storeLon,
                address.Latitude.Value, address.Longitude.Value
            );

            // Công thức: Phí cố định + (Số km * Phí mỗi km)
            decimal fixedFee = store.ShippingFeeFixed ?? 0;
            decimal perKmFee = store.ShippingFeePerKm ?? 5000;

            return fixedFee + (decimal)distanceKm * perKmFee;
        }

        // =========================================================================
        // 5. CÁC HÀM GET & SUPPORT KHÁC
        // =========================================================================
        public async Task<OrderReadDto> GetOrderByIdAsync(long id)
        {
            // Cần Include rất nhiều bảng liên quan
            var order = await _unitOfWork.Orders.Find(o => o.Id == id)
                .Include(o => o.Store)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product) // Món chính -> Product
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)    // Món chính -> Size
                .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem).ThenInclude(ti => ti.Product) // Topping -> Product
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Table)
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync();

            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            return _mapper.Map<OrderReadDto>(order);
        }

        public async Task<PagedResult<OrderReadDto>> GetMyOrdersAsync(int userId, PagingRequest request)
        {
            var query = _unitOfWork.Orders.Find(o => o.UserId == userId);

            // Đếm tổng
            int totalRow = await query.CountAsync();

            // Phân trang & Sort (Mới nhất lên đầu)
            var data = await query.OrderByDescending(o => o.CreatedAt)
                                  .Skip((request.PageIndex - 1) * request.PageSize)
                                  .Take(request.PageSize)
                                  .Include(o => o.Store) // Include nhẹ để hiển thị list
                                  .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                  .ToListAsync();

            var dtos = _mapper.Map<List<OrderReadDto>>(data);

            return new PagedResult<OrderReadDto>(dtos, totalRow, request.PageIndex, request.PageSize);
        }

        //// --- Placeholder Implementations ---
        //public Task<OrderReadDto> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> CancelOrderAsync(long orderId, int? userId, OrderCancelDto cancelDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> AssignShipperAsync(long orderId, int shipperId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> VerifyPickupCodeAsync(long orderId, string code)
        //{
        //    throw new NotImplementedException();
        //}

        // Helper
        private string GenerateOrderCode()
        {
            // Format: ORD-{yyyyMMdd}-{Random4So}
            return $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        // =========================================================================
        // 6. LỌC ĐƠN HÀNG (CHO QUẢN LÝ / STAFF)
        // =========================================================================
        public async Task<PagedResult<OrderReadDto>> GetOrdersByFilterAsync(OrderFilterDto filter)
        {
            // 1. Khởi tạo Query (Chưa chạy xuống DB)
            var query = _unitOfWork.Orders.Find(x => true); // Lấy tất cả

            // 2. Áp dụng các điều kiện lọc (Filter)
            if (filter.StoreId.HasValue)
            {
                query = query.Where(o => o.StoreId == filter.StoreId);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(o => o.Status == filter.Status);
            }

            if (filter.FromDate.HasValue)
            {
                // So sánh ngày bắt đầu (Lấy phần Date để chính xác)
                var fromDate = filter.FromDate.Value.Date;
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date >= fromDate);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date;
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date <= toDate);
            }

            // 3. Đếm tổng số bản ghi (phục vụ phân trang)
            int totalRecords = await query.CountAsync();

            // 4. Phân trang & Sắp xếp & Include dữ liệu
            var items = await query.OrderByDescending(o => o.CreatedAt) // Mới nhất lên đầu
                                   .Skip((filter.PageIndex - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .Include(o => o.Store)           // Lấy tên quán
                                   .Include(o => o.User)            // Lấy tên khách
                                   .Include(o => o.DeliveryAddress) // Lấy địa chỉ giao
                                   .Include(o => o.Shipper)         // Lấy tên shipper
                                   .Include(o => o.Table)           // Lấy tên bàn
                                   .Include(o => o.OrderItems).ThenInclude(oi => oi.Product) // Lấy món
                                   .ToListAsync();

            // 5. Map sang DTO
            var dtos = _mapper.Map<List<OrderReadDto>>(items);

            return new PagedResult<OrderReadDto>(dtos, totalRecords, filter.PageIndex, filter.PageSize);
        }

        // =========================================================================
        // 7. THỐNG KÊ NHANH (DOANH THU & TRẠNG THÁI)
        // =========================================================================
        public async Task<OrderQuickStatsDto> GetQuickStatsAsync(int? storeId, DateTime date)
        {
            var targetDate = date.Date;

            // --- A. Thống kê theo NGÀY CHỈ ĐỊNH (Doanh thu & Số đơn hôm nay) ---
            var todayQuery = _unitOfWork.Orders.Find(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == targetDate);

            if (storeId.HasValue)
            {
                todayQuery = todayQuery.Where(o => o.StoreId == storeId);
            }

            // Tính tổng doanh thu (Chỉ tính đơn đã hoàn thành hoặc đã thanh toán)
            // Lưu ý: Tùy logic quán, có thể tính cả đơn Confirm. Ở đây mình tính đơn Completed.
            var todayRevenue = await todayQuery
                .Where(o => o.Status == OrderStatusEnum.Completed)
                .SumAsync(o => o.GrandTotal);

            var todayOrdersCount = await todayQuery.CountAsync();


            // --- B. Thống kê theo THỜI GIAN THỰC (Backlog - Việc cần làm ngay) ---
            // Phần này không lọc theo ngày, mà lọc theo trạng thái hiện tại (Đơn đang treo)
            var backlogQuery = _unitOfWork.Orders.Find(x => true);

            if (storeId.HasValue)
            {
                backlogQuery = backlogQuery.Where(o => o.StoreId == storeId);
            }

            // Đếm đơn chờ xác nhận (New) và đang chế biến (Confirmed/Preparing)
            var pendingCount = await backlogQuery
                .CountAsync(o => o.Status == OrderStatusEnum.New ||
                                 o.Status == OrderStatusEnum.Confirmed ||
                                 o.Status == OrderStatusEnum.Preparing);

            // Đếm đơn đang giao (Delivering)
            var shippingCount = await backlogQuery
                .CountAsync(o => o.Status == OrderStatusEnum.Delivering);


            // --- C. Trả về kết quả ---
            return new OrderQuickStatsDto
            {
                TodayRevenue = todayRevenue,
                TodayOrders = todayOrdersCount,
                PendingOrders = pendingCount,
                ShippingOrders = shippingCount
            };
        }

        // =========================================================================
        // 8. CẬP NHẬT TRẠNG THÁI (DUYỆT, NẤU, GIAO...)
        // =========================================================================
        public async Task<OrderReadDto> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");

            // Validate logic chuyển trạng thái (cơ bản)
            if (order.Status == OrderStatusEnum.Cancelled || order.Status == OrderStatusEnum.Completed)
            {
                throw new Exception("Không thể cập nhật trạng thái cho đơn đã hoàn thành hoặc đã hủy.");
            }

            // Cập nhật
            order.Status = newStatus;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return await GetOrderByIdAsync(orderId);
        }

        // =========================================================================
        // 9. HỦY ĐƠN HÀNG (KHÁCH HÀNG / QUẢN LÝ)
        // =========================================================================
        public async Task<bool> CancelOrderAsync(long orderId, int? userId, OrderCancelDto cancelDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");

            // 1. Chỉ được hủy khi đơn còn mới
            if (order.Status != OrderStatusEnum.New && order.Status != OrderStatusEnum.PendingPayment)
            {
                throw new Exception("Đơn hàng đã được tiếp nhận, không thể hủy. Vui lòng liên hệ cửa hàng.");
            }

            // 2. Cập nhật thông tin hủy
            order.Status = OrderStatusEnum.Cancelled;
            order.CancelReason = cancelDto.Reason;
            order.CancelNote = cancelDto.Note;
            order.CancelledByUserId = userId ?? 0;

            // 3. Hoàn lại Voucher (nếu có)
            if (order.UserVoucherId.HasValue)
            {
                var userVoucher = await _unitOfWork.Repository<UserVoucher>().GetByIdAsync(order.UserVoucherId.Value);
                if (userVoucher != null)
                {
                    userVoucher.Status = UserVoucherStatusEnum.Unused;
                    userVoucher.UsedDate = null;
                    userVoucher.OrderIdUsed = null;
                    _unitOfWork.Repository<UserVoucher>().Update(userVoucher);
                }
            }

            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        // =========================================================================
        // 10. GÁN SHIPPER (VẬN HÀNH)
        // =========================================================================
        public async Task<bool> AssignShipperAsync(long orderId, int shipperId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");

            if (order.OrderType != OrderTypeEnum.Delivery)
                throw new Exception("Chỉ có thể gán Shipper cho đơn giao hàng.");

            var shipper = await _unitOfWork.Repository<User>().GetByIdAsync(shipperId);
            if (shipper == null) throw new Exception("Shipper không hợp lệ.");

            order.ShipperId = shipperId;
            order.Status = OrderStatusEnum.Delivering; // Chuyển trạng thái luôn

            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        // =========================================================================
        // 11. XÁC THỰC MÃ LẤY ĐỒ (AT COUNTER)
        // =========================================================================
        public async Task<bool> VerifyPickupCodeAsync(long orderId, string code)
        {
            // Tìm đơn theo ID và Code
            var order = await _unitOfWork.Orders.Find(o => o.Id == orderId && o.PickupCode == code).FirstOrDefaultAsync();

            if (order == null) return false;

            // Nếu đúng mã -> Hoàn thành đơn
            order.Status = OrderStatusEnum.Received;
            _unitOfWork.Orders.Update(order);

            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}