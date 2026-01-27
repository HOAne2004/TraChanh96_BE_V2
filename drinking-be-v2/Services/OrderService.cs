using AutoMapper;
using drinking_be.Data; // Chứa IUnitOfWork
using drinking_be.Domain.Orders;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.NotificationDtos;
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
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly INotificationService _notificationService;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderPaymentService orderPaymentService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderPaymentService = orderPaymentService;
            _notificationService = notificationService;
        }
        public class AppException : Exception
        {
            public AppException(string message) : base(message) { }
        }

        // =========================================================================
        // 1. TẠO ĐƠN GIAO HÀNG (DELIVERY)
        // =========================================================================
        public async Task<OrderReadDto> CreateDeliveryOrderAsync(int? userId, DeliveryOrderCreateDto dto)
        {
            // 1. Validate Store
            var store = await _unitOfWork.Stores.Find(s => s.Id == dto.StoreId)
                                            .Include(s => s.Address)
                                            .FirstOrDefaultAsync();
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
            
            var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(dto.PaymentMethodId);
            if (paymentMethod != null &&
            (paymentMethod.PaymentType == PaymentTypeEnum.BankTransfer ||
            paymentMethod.PaymentType == PaymentTypeEnum.EWallet))
                {
                    order.Status = OrderStatusEnum.PendingPayment;
                }
            // 4. Xử lý danh sách món & Tính tiền hàng (SubTotal)
            order.TotalAmount = await ProcessOrderItemsAsync(order, dto.Items);
            order.RecipientName = address.RecipientName;
            order.RecipientPhone = address.RecipientPhone;
            order.ShippingAddress = !string.IsNullOrWhiteSpace(address.FullAddress)
                ? address.FullAddress
                : $"{address.AddressDetail}, {address.Commune}, {address.District}, {address.Province}";

            // 5. Tính phí Ship (Dùng Haversine)
            order.ShippingFee = CalculateShippingFeeLogic(store, address);

            // 6. Tính tổng tiền & Voucher (Tạm thời chưa trừ Voucher phức tạp)
            order.DiscountAmount = 0; // TODO: Gọi VoucherService để tính
            order.GrandTotal = order.TotalAmount + (order.ShippingFee ?? 0) - (order.DiscountAmount ?? 0);

            // 7. Tính điểm thưởng (Ví dụ: 1% giá trị đơn hàng)
            order.CoinsEarned = (int)(order.GrandTotal * 0.01m);

            // 8. Lưu xuống DB
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            var adminIds = await _unitOfWork.Repository<User>()
        .GetAllAsync(u => u.RoleId == UserRoleEnum.Admin || u.RoleId == UserRoleEnum.Manager || u.RoleId == UserRoleEnum.Staff);

            foreach (var admin in adminIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = admin.Id,
                    Title = "Có đơn hàng mới! 🔔",
                    Content = $"Đơn mới #{order.OrderCode} vừa được tạo. Giá trị: {order.GrandTotal:N0}đ",
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = order.OrderCode
                });
            }


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

            var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(dto.PaymentMethodId);

            order.Status = OrderStatusEnum.New;
            if (paymentMethod != null &&
               (paymentMethod.PaymentType == PaymentTypeEnum.BankTransfer ||
                paymentMethod.PaymentType == PaymentTypeEnum.EWallet))
            {
                order.Status = OrderStatusEnum.PendingPayment;
            }

            // 4. Xử lý món
            order.TotalAmount = await ProcessOrderItemsAsync(order, dto.Items);

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

            var adminIds = await _unitOfWork.Repository<User>()
        .GetAllAsync(u => u.RoleId == UserRoleEnum.Admin || u.RoleId == UserRoleEnum.Manager || u.RoleId == UserRoleEnum.Staff);

            foreach (var admin in adminIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = admin.Id,
                    Title = "Có đơn hàng mới! 🔔",
                    Content = $"Đơn mới #{order.OrderCode} vừa được tạo. Giá trị: {order.GrandTotal:N0}đ",
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = order.OrderCode
                });
            }

            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // [NEW] TẠO ĐƠN ĐẾN LẤY (PICKUP) - KHÁCH TỰ ĐẶT
        // =========================================================================
        public async Task<OrderReadDto> CreatePickupOrderAsync(int userId, PickupOrderCreateDto dto)
        {
            // 1. Validate Store
            var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId);
            if (store == null) throw new KeyNotFoundException("Cửa hàng không tồn tại.");
            if (store.Status != StoreStatusEnum.Active) throw new AppException("Cửa hàng đang đóng cửa.");

            // 2. Validate Payment
            var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(dto.PaymentMethodId);
            if (paymentMethod == null) throw new AppException("Phương thức thanh toán không hợp lệ.");

            // Chặn thanh toán sau (COD/Cash)
            // Lưu ý: Sửa Exception -> AppException để trả về 400 thay vì 500
            if (paymentMethod.PaymentType == PaymentTypeEnum.COD)
            {
                throw new AppException("Đơn hàng 'Đến lấy' yêu cầu thanh toán trước (Chuyển khoản/Ví điện tử).");
            }

            // 🟢 3. XỬ LÝ TIMEZONE (QUAN TRỌNG)
            // Frontend gửi lên là UTC (do toISOString), cần check xem nó có nhỏ hơn hiện tại (UTC) không
            if (dto.PickupTime < DateTime.UtcNow)
                throw new AppException("Thời gian lấy hàng không hợp lệ (phải lớn hơn hiện tại).");

            // Check giờ mở cửa của quán
            if (store.OpenTime.HasValue && store.CloseTime.HasValue)
            {
                // Chuyển giờ khách chọn (UTC) sang giờ Việt Nam (UTC+7) để so sánh với giờ mở cửa của quán
                // Cách đơn giản nhất và chạy đúng trên mọi môi trường (Windows/Linux) là cộng cứng 7 tiếng
                // Hoặc dùng TimeZoneInfo nếu muốn chuẩn chỉ hơn.

                var pickupTimeVN = dto.PickupTime.ToUniversalTime().AddHours(7);
                var pickupTimeOfDay = pickupTimeVN.TimeOfDay;

                if (pickupTimeOfDay < store.OpenTime.Value || pickupTimeOfDay > store.CloseTime.Value)
                {
                    throw new AppException($"Cửa hàng chỉ mở cửa từ {store.OpenTime.Value:hh\\:mm} đến {store.CloseTime.Value:hh\\:mm}. Vui lòng chọn giờ khác.");
                }
            }

            // 4. Khởi tạo Order
            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            order.OrderCode = GenerateOrderCode();
            order.OrderType = OrderTypeEnum.Pickup;

            // AN TOÀN: Vì bắt buộc thanh toán trước, mặc định set là PendingPayment
            order.Status = OrderStatusEnum.PendingPayment;

            order.PickupCode = GeneratePickupCode();
            order.PickupTime = dto.PickupTime; // Lưu UTC vào DB là chuẩn, khi hiển thị ra FE lại convert sang Local sau

            // 5. Xử lý món ăn
            order.TotalAmount = await ProcessOrderItemsAsync(order, dto.Items);

            // 6. Pickup settings
            order.ShippingFee = 0;
            order.DeliveryAddressId = null; // ⚠️ Đảm bảo DB cột này cho phép NULL

            // 7. Tính tổng
            order.DiscountAmount = 0; // TODO: Voucher logic sau này
            order.GrandTotal = order.TotalAmount - (order.DiscountAmount ?? 0);
            order.CoinsEarned = (int)(order.GrandTotal * 0.01m);

            // 8. Lưu DB
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            // 9. Gửi thông báo (Gọi hàm helper)
            await NotifyStaffAsync(order);

            return await GetOrderByIdAsync(order.Id);
        }
        // --- HELPER ---
        private string GeneratePickupCode()
        {
            // Sinh chuỗi số ngẫu nhiên 6 ký tự (VD: 839201) dễ đọc cho nhân viên
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Tách hàm gửi Noti để code gọn hơn
        private async Task NotifyStaffAsync(Order order)
        {
            var adminIds = await _unitOfWork.Repository<User>()
                 .GetAllAsync(u => u.RoleId == UserRoleEnum.Admin || u.RoleId == UserRoleEnum.Manager || u.RoleId == UserRoleEnum.Staff);

            foreach (var admin in adminIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = admin.Id,
                    Title = "Có đơn qua lấy mới! 🔔",
                    Content = $"Khách hẹn lấy lúc {order.PickupTime:HH:mm}. Mã đơn #{order.OrderCode}. Giá trị: {order.GrandTotal:N0}đ",
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = order.OrderCode
                });
            }
        }


        // =========================================================================
        // 3. PRIVATE: XỬ LÝ MÓN ĂN & TOPPING (LOGIC CỐT LÕI)
        // =========================================================================
        private async Task<decimal> ProcessOrderItemsAsync(Order order, List<OrderItemCreateDto> itemsDto)
        {
            decimal totalAmount = 0;

            if (itemsDto == null || !itemsDto.Any())
                throw new AppException("Danh sách món không được rỗng.");

            var productIds = itemsDto
                .Select(i => i.ProductId)
                .Concat(itemsDto.SelectMany(i => i.Toppings.Select(t => t.ProductId)))
                .Distinct()
                .ToList();

            var products = await _unitOfWork.Products.GetQueryable()
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductStores)
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var itemDto in itemsDto)
            {
                if (itemDto.Quantity <= 0)
                    throw new AppException("Số lượng sản phẩm không hợp lệ.");

                if (!products.TryGetValue(itemDto.ProductId, out var product))
                    throw new AppException("Sản phẩm không tồn tại.");

                if (!product.ProductStores.Any(ps => ps.StoreId == order.StoreId))
                    throw new AppException("Sản phẩm không thuộc cửa hàng này.");

                var basePrice = product.BasePrice;
                var unitPrice = basePrice;

                short? sizeId = null;
                string? sizeName = null;
                decimal sizePrice = 0;

                if (itemDto.SizeId.HasValue)
                {
                    var size = product.ProductSizes
                        .FirstOrDefault(s => s.SizeId == itemDto.SizeId.Value);

                    if (size == null)
                        throw new AppException("Size không hợp lệ.");

                    unitPrice += size.PriceOverride.GetValueOrDefault(0);

                    sizeId = size.SizeId;
                    sizeName = size.Size.Label;
                    sizePrice = size.PriceOverride.GetValueOrDefault(0);
                }


                var mainItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.ImageUrl,
                    Quantity = itemDto.Quantity,
                    BasePrice = basePrice,
                    SizeId = sizeId,
                    SizeName = sizeName,
                    SizePrice = sizePrice, // ✅ BỔ SUNG
                    SugarLevel = itemDto.SugarLevel,
                    IceLevel = itemDto.IceLevel,
                    Note = itemDto.Note,
                    Order = order
                };

                decimal toppingUnitTotal = 0;

                foreach (var toppingDto in itemDto.Toppings)
                {
                    if (!products.TryGetValue(toppingDto.ProductId, out var toppingProduct))
                        throw new AppException("Topping không tồn tại.");

                    if (!toppingProduct.ProductStores.Any(ps => ps.StoreId == order.StoreId))
                        throw new AppException("Topping không thuộc cửa hàng này.");

                    toppingUnitTotal += toppingProduct.BasePrice;

                    var toppingItem = new OrderItem
                    {
                        ProductId = toppingProduct.Id,
                        ProductName = toppingProduct.Name,
                        ProductImage = toppingProduct.ImageUrl,
                        Quantity = mainItem.Quantity,
                        BasePrice = toppingProduct.BasePrice,
                        FinalPrice = toppingProduct.BasePrice * mainItem.Quantity,
                        ParentItem = mainItem,
                        ParentItemId = mainItem.Id,
                        Order = order
                    };

                    mainItem.InverseParentItem.Add(toppingItem);
                }

                mainItem.FinalPrice = (unitPrice + toppingUnitTotal) * mainItem.Quantity;
                totalAmount += mainItem.FinalPrice;

                order.OrderItems.Add(mainItem);
            }

            return totalAmount;
        }

        // =========================================================================
        // 4. PRIVATE: TÍNH PHÍ SHIP (HAVERSINE)
        // =========================================================================
        // Trong OrderService.cs

        private decimal CalculateShippingFeeLogic(Store store, Address address)
        {
            // 1. Kiểm tra tọa độ khách 
            if (!address.Latitude.HasValue || !address.Longitude.HasValue)
                return store.ShippingFeeFixed ?? 15000;

            // 2. Kiểm tra tọa độ quán
            if (store.Address == null || !store.Address.Latitude.HasValue || !store.Address.Longitude.HasValue)
                return store.ShippingFeeFixed ?? 15000;

            // 3. Tính khoảng cách
            double distanceKm = DistanceUtils.CalculateDistanceKm(
                store.Address.Latitude.Value, store.Address.Longitude.Value,
                address.Latitude.Value, address.Longitude.Value
            );

            // Lấy giới hạn riêng của quán, nếu chưa set thì lấy 20km
            double limitKm = store.DeliveryRadius > 0 ? store.DeliveryRadius : 20;

            if (distanceKm > limitKm)
            {
                // Ném lỗi để Controller bắt được và trả về 400 cho FE
                throw new AppException($"Khoảng cách {distanceKm:F1}km vượt quá giới hạn giao hàng ({limitKm}km) của quán.");
            }

            // 4. Tính tiền (Code cũ)
            decimal perKmFee = store.ShippingFeePerKm ?? 5000;
            decimal rawFee = (decimal)distanceKm * perKmFee;

            return Math.Ceiling(rawFee / 1000) * 1000;
        }

        // =========================================================================
        // 5. CÁC HÀM GET & SUPPORT KHÁC
        // =========================================================================
        public async Task<OrderReadDto> GetOrderByIdAsync(long id)
        {
            // Cần Include rất nhiều bảng liên quan
            var order = await _unitOfWork.Orders.Find(o => o.Id == id)
                .Include(o => o.Store)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)    // Món chính -> Size
                .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem).ThenInclude(ti => ti.InverseParentItem) // Topping -> Product
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Table)
                .Include(o => o.Shipper)
                .Include(o =>o.OrderPayments)
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
                                  .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem)
                                  .Include(o => o.OrderPayments)
                                  .ToListAsync();

            var dtos = _mapper.Map<List<OrderReadDto>>(data);

            return new PagedResult<OrderReadDto>(dtos, totalRow, request.PageIndex, request.PageSize);
        }

        // Helper
        private string GenerateOrderCode() => $"ORD-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        // =========================================================================
        // 6. LỌC ĐƠN HÀNG (CHO QUẢN LÝ / STAFF)
        // =========================================================================
        public async Task<PagedResult<OrderReadDto>> GetOrdersByFilterAsync(OrderFilterDto filter)
        {
            // 1. Khởi tạo Query
            var query = _unitOfWork.Orders.GetQueryable()
                .Include(o => o.Store)           // Lấy tên quán
                .Include(o => o.User)            // Lấy tên khách
                .Include(o => o.Shipper)         // Lấy tên shipper
                .Include(o => o.Table)           // Lấy tên bàn
                                                 // Nếu cần lấy chi tiết món để hiển thị thì include, không thì thôi để query nhanh hơn
                                                 // .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem) 
                .AsQueryable();

            // 2. Áp dụng các điều kiện lọc (Filter)

            // --- A. Lọc Thùng Rác (IsDeleted) ---
            if (filter.IsDeleted)
            {
                // Xem thùng rác -> Bỏ qua Global Filter, chỉ lấy cái đã xóa
                query = query.IgnoreQueryFilters().Where(o => o.DeletedAt != null);
            }
            else
            {
                // Xem bình thường -> Chỉ lấy cái chưa xóa
                query = query.Where(o => o.DeletedAt == null);
            }

            // --- B. Lọc theo Store/Status/Date ---
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
                var fromDate = filter.FromDate.Value.Date;
                query = query.Where(o => o.CreatedAt >= fromDate); // Dùng >= thay vì .Date để tận dụng index nếu có
            }

            if (filter.ToDate.HasValue)
            {
                // Muốn lấy hết ngày đó thì phải là <= cuối ngày hoặc < ngày hôm sau
                // Cách đơn giản nhất:
                var toDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.CreatedAt < toDate);
            }

            // 🟢 [QUAN TRỌNG] C. LOGIC TÌM KIẾM (KEYWORD) ---
            // Đây là phần bạn bị thiếu ở đoạn code trên
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                string k = filter.Keyword.Trim().ToLower();

                // Tìm trong: Mã đơn OR Tên người nhận OR SĐT người nhận
                query = query.Where(o =>
                    // 1. Tìm theo Mã đơn
                    o.OrderCode.ToLower().Contains(k) ||

                    // 2. Tìm theo Tên người nhận (Ship)
                    (o.RecipientName != null && o.RecipientName.ToLower().Contains(k)) ||

                    // 3. Tìm theo SĐT người nhận
                    (o.RecipientPhone != null && o.RecipientPhone.Contains(k)) ||

                    // 4. Tìm theo Tên tài khoản (User) - FIX LỖI 500 TẠI ĐÂY
                    // Phải truy cập vào bảng User và kiểm tra null
                    (o.User != null && o.User.Username.ToLower().Contains(k))
        );
            }

            // 3. Đếm tổng số bản ghi (trước khi phân trang)
            int totalRecords = await query.CountAsync();

            // 4. Phân trang & Sắp xếp
            var items = await query.OrderByDescending(o => o.CreatedAt) // Mới nhất lên đầu
                                   .Skip((filter.PageIndex - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync();

            // 5. Map sang DTO
            var dtos = _mapper.Map<List<OrderReadDto>>(items);

            return new PagedResult<OrderReadDto>(dtos, totalRecords, filter.PageIndex, filter.PageSize);
        }

        // =========================================================================
        // 7. THỐNG KÊ NHANH (DOANH THU & TRẠNG THÁI)
        // =========================================================================
        public async Task<OrderQuickStatsDto> GetQuickStatsAsync(int? storeId, DateTime? dateInput)
        {
            // 1. XỬ LÝ TIMEZONE & DATE
            TimeZoneInfo vnTimeZone;
            try
            {
                vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch
            {
                vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }

            // Xác định "Ngày mục tiêu" (Target Date)
            DateTime targetDateVn;

            if (dateInput.HasValue)
            {
                // Nếu FE gửi lên, ta dùng ngày đó (Lưu ý: FE nên gửi yyyy-MM-dd để tránh bị +/- giờ do UTC)
                // .Date để bỏ phần giờ phút nếu có
                targetDateVn = dateInput.Value.Date;
            }
            else
            {
                // Nếu không gửi (null), lấy ngày hiện tại theo giờ VN
                targetDateVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;
            }

            // Tính khung giờ Start/End theo UTC để query DB
            var startOfDayVn = targetDateVn;
            var endOfDayVn = targetDateVn.AddDays(1).AddTicks(-1);

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayVn, vnTimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDayVn, vnTimeZone);

            // 2. KHỞI TẠO QUERY CƠ BẢN (Dùng chung để code gọn hơn)
            var baseQuery = _unitOfWork.Orders.GetQueryable();

            if (storeId.HasValue)
            {
                baseQuery = baseQuery.Where(o => o.StoreId == storeId);
            }

            // --- A. THỐNG KÊ THEO NGÀY CHỌN (Today/Target Date) ---
            var dailyQuery = baseQuery.Where(o => o.CreatedAt >= startUtc && o.CreatedAt <= endUtc);

            // Tính doanh thu ngày (Bổ sung Received để khớp với logic đơn thành công)
            var todayRevenue = await dailyQuery
                .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                .SumAsync(o => (decimal?)o.GrandTotal) ?? 0;

            var todayOrdersCount = await dailyQuery.CountAsync();

            // --- B. THỐNG KÊ TOÀN THỜI GIAN (All Time) ---

            // Tính tổng doanh thu trọn đời (Bổ sung Received)
            var totalRevenueAllTime = await baseQuery
                .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                .SumAsync(o => (decimal?)o.GrandTotal) ?? 0;

            // Tính tổng đơn thành công trọn đời
            var totalCompletedOrders = await baseQuery
                .CountAsync(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received);

            // --- C. BACKLOG (VIỆC CẦN LÀM NGAY) ---
            // Backlog phản ánh trạng thái "Live" nên không lọc theo ngày

            var pendingCount = await baseQuery
                .CountAsync(o => o.Status == OrderStatusEnum.New ||
                                 o.Status == OrderStatusEnum.Confirmed ||
                                 o.Status == OrderStatusEnum.Preparing);

            var shippingCount = await baseQuery
                .CountAsync(o => o.Status == OrderStatusEnum.Delivering);

            return new OrderQuickStatsDto
            {
                TodayRevenue = todayRevenue,
                TotalRevenueAllTime = totalRevenueAllTime,
                TotalCompletedOrders = totalCompletedOrders,
                TodayOrders = todayOrdersCount,
                PendingOrders = pendingCount,
                ShippingOrders = shippingCount
            };
        }
        // =========================================================================
        // 8. CẬP NHẬT TRẠNG THÁI (DUYỆT, NẤU, GIAO...)
        // =========================================================================
        public async Task<OrderReadDto> UpdateOrderStatusAsync(
            long orderId,
            OrderStatusEnum newStatus,
            UserRoleEnum actorRole)
        {
            // 1. Load order
            var order = await _unitOfWork.Orders.Find(o => o.Id == orderId)
                .Include(o => o.PaymentMethod) // 👈 Quan trọng để AutoConfirmPayment hoạt động
                .Include(o => o.User)          // 👈 Quan trọng để gửi Notification
                .FirstOrDefaultAsync()
                ?? throw new AppException("Đơn hàng không tồn tại.");

            // 2. Build payment snapshot (🔑 từ PaymentService)
            var paymentSnapshot =
                await _orderPaymentService.BuildPaymentSnapshotAsync(orderId);

            // 3. Validate state transition (DOMAIN RULE)
            OrderStateMachine.ValidateTransition(
                from: order.Status,
                to: newStatus,
                payment: paymentSnapshot,
                actor: actorRole,
                orderGrandTotal: order.GrandTotal
            );
            if (newStatus == OrderStatusEnum.Completed || newStatus == OrderStatusEnum.Received)
            {
                // Kiểm tra: Nếu chưa thanh toán đủ
                if (!paymentSnapshot.IsFullyPaid(order.GrandTotal))
                {
                    decimal amountMissing = order.GrandTotal - paymentSnapshot.PaidAmount;

                    // Kiểm tra PaymentMethod có tồn tại không để tránh lỗi Null
                    if (order.PaymentMethod != null)
                    {
                        // Gọi hàm AutoConfirmPaymentAsync vừa viết ở trên
                        await _orderPaymentService.AutoConfirmPaymentAsync(
                            order.Id,
                            order.PaymentMethod.Id,      // ID phương thức (VD: 1 - COD)
                            order.PaymentMethod.Name,    // Tên phương thức (VD: "Thanh toán khi nhận hàng")
                            amountMissing,               // Số tiền thu
                            "Hệ thống tự động xác nhận thanh toán khi Hoàn tất đơn." // Ghi chú
                        );
                    }
                }
            }

            // 4. Update state
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            // 5. Gửi thông báo (Notification)
            if (order.UserId.HasValue)
            {
                string title = "Cập nhật đơn hàng";
                string content = "";

                switch (newStatus)
                {
                    case OrderStatusEnum.Confirmed:
                        content = $"Đơn hàng #{order.OrderCode} đã được xác nhận và đang được pha chế 🥤.";
                        break;
                    case OrderStatusEnum.Delivering:
                        content = $"Shipper đang giao đơn hàng #{order.OrderCode} đến bạn 🛵. Vui lòng để ý điện thoại.";
                        break;
                    case OrderStatusEnum.Completed:
                        content = $"Đơn hàng #{order.OrderCode} đã hoàn thành. Chúc bạn ngon miệng! ⭐";
                        break;
                    case OrderStatusEnum.Received: // Tại quầy
                        content = $"Món của bạn đã sẵn sàng! Vui lòng tới quầy nhận đơn #{order.OrderCode}.";
                        break;
                        // Các case khác tùy chọn...
                }

                if (!string.IsNullOrEmpty(content))
                {
                    await _notificationService.CreateAsync(new NotificationCreateDto
                    {
                        UserId = order.UserId.Value,
                        Title = title,
                        Content = content,
                        Type = NotificationTypeEnum.Order,
                        ReferenceId = order.OrderCode
                    });
                }
            }
            // 6. Return updated order
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
            // ⚠️ SỬA: Thêm chấm phẩy và đảm bảo hàm GetEnumDescription tồn tại (xem bên dưới)
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

            // ⚠️ SỬA QUAN TRỌNG: Lưu kết quả vào biến success chứ KHÔNG return ngay
            var success = await _unitOfWork.CompleteAsync() > 0;

            // Logic thông báo chỉ chạy khi lưu DB thành công
            if (success && order.UserId.HasValue)
            {
                // --- 🟢 LOGIC THÔNG BÁO HOÀN TIỀN ---
                string notiContent;
                string notiTitle = "Đơn hàng đã bị hủy";

                // Kiểm tra xem đơn đã thanh toán chưa
                var paymentSnapshot = await _orderPaymentService.BuildPaymentSnapshotAsync(orderId);
                bool isPaid = paymentSnapshot.IsFullyPaid(order.GrandTotal);

                if (isPaid)
                {
                    notiContent = $"Đơn hàng #{order.OrderCode} đã hủy. Vì bạn đã thanh toán, cửa hàng sẽ liên hệ hoàn tiền trong vòng 24h.";
                }
                else
                {
                    notiContent = $"Đơn hàng #{order.OrderCode} đã hủy thành công.";
                }

                // Gửi thông báo
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = order.UserId.Value,
                    Title = notiTitle,
                    Content = notiContent,
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = order.OrderCode
                });
            }

            return success;
        }

        // =========================================================================
        // 10. GÁN SHIPPER (VẬN HÀNH)
        // =========================================================================
        public async Task<bool> AssignShipperAsync(long orderId, int shipperId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");
            
            if (order.OrderType != OrderTypeEnum.Delivery)
                throw new AppException("Đơn hàng 'Đến lấy' không cần gán Shipper.");
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
            // Tìm đơn theo ID và Code, đảm bảo Status chưa hoàn thành
            var order = await _unitOfWork.Orders.Find(o =>
                o.Id == orderId &&
                o.PickupCode == code
            ).FirstOrDefaultAsync();

            // 🟢 SỬA: Ném lỗi thay vì return false
            if (order == null)
                throw new KeyNotFoundException("Mã lấy đồ không đúng hoặc đơn hàng không tồn tại.");

            if (order.Status == OrderStatusEnum.Received || order.Status == OrderStatusEnum.Completed)
                throw new AppException("Đơn hàng này đã được nhận rồi.");

            // Nếu đúng mã -> Hoàn thành đơn
            order.Status = OrderStatusEnum.Received;
            order.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian

            _unitOfWork.Orders.Update(order);

            return await _unitOfWork.CompleteAsync() > 0;
        }

        // =========================================================================
        // 12. LẤY ĐƠN THEO ORDER CODE
        // ========================================================================
        public async Task<OrderReadDto> GetOrderByOrderCodeAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders.Find(o => o.OrderCode == orderCode)
                .Include(o => o.Store)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)    // Món chính -> Size
                .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem).ThenInclude(ti => ti.InverseParentItem) // Topping -> Product
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Table)
                .Include(o => o.Shipper)
                .Include(o => o.OrderPayments)
                .FirstOrDefaultAsync();
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
            return _mapper.Map<OrderReadDto>(order);
        }

        // 
        // Implement hàm tính phí (Tách logic cũ ra)
        public async Task<decimal> CalculateShippingFeeAsync(int storeId, long addressId)
        {
            // 1. Lấy thông tin Store (Kèm Address)
            var store = await _unitOfWork.Stores.Find(s => s.Id == storeId)
                                                .Include(s => s.Address) // Quan trọng
                                                .FirstOrDefaultAsync();
            if (store == null) throw new KeyNotFoundException("Cửa hàng không tồn tại.");

            // 2. Lấy thông tin Address
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);
            if (address == null) throw new KeyNotFoundException("Địa chỉ không tồn tại.");

            // 3. Gọi hàm logic private để tính
            return CalculateShippingFeeLogic(store, address);
        }

        // =========================================================================
        // [NEW] 13. XÓA MỀM & KHÔI PHỤC
        // =========================================================================

        public async Task<bool> SoftDeleteOrderAsync(long id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return false;

            // Logic nghiệp vụ: Chỉ cho xóa đơn đã Hủy hoặc đã Hoàn thành lâu? 
            // Hoặc cho xóa tất cả tùy quyền Admin. Ở đây cho phép xóa tất cả để vào thùng rác.

            order.DeletedAt = DateTime.UtcNow; // Đánh dấu xóa
            _unitOfWork.Orders.Update(order);

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> RestoreOrderAsync(long id)
        {
            // Phải dùng IgnoreQueryFilters để tìm thấy đơn đã bị ẩn
            var order = await _unitOfWork.Orders.GetQueryable()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            order.DeletedAt = null; // Khôi phục
            _unitOfWork.Orders.Update(order);

            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
