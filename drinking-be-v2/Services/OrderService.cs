using AutoMapper;
using drinking_be.Data;
using drinking_be.Domain.Orders;
using drinking_be.Domain.Services;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.NotificationDtos;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly INotificationService _notificationService;

        // Domain Services (DDD Lite)
        private readonly ShippingCalculator _shippingCalculator;
        private readonly OrderPriceCalculator _priceCalculator;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOrderPaymentService orderPaymentService,
            INotificationService notificationService,
            ShippingCalculator shippingCalculator,
            OrderPriceCalculator priceCalculator
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderPaymentService = orderPaymentService;
            _notificationService = notificationService;
            _shippingCalculator = shippingCalculator;
            _priceCalculator = priceCalculator;
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
            // 1. Validate Store & Address
            var store = await ValidateStoreAsync(dto.StoreId);
            var address = await _unitOfWork.Addresses.GetByIdAsync(dto.DeliveryAddressId)
                          ?? throw new KeyNotFoundException("Địa chỉ giao hàng không tồn tại.");

            // 2. Init Order
            var order = InitializeOrder(dto, userId);
            order.RecipientName = address.RecipientName;
            order.RecipientPhone = address.RecipientPhone;
            order.ShippingAddress = !string.IsNullOrWhiteSpace(address.FullAddress)
                ? address.FullAddress
                : $"{address.AddressDetail}, {address.Commune}, {address.District}, {address.Province}";

            if (!dto.PaymentMethodId.HasValue)
                throw new AppException("Phương thức thanh toán không hợp lệ.");
            await ValidatePaymentMethodAsync(order, dto.PaymentMethodId.Value);

            // 3. Calculate Items & Price (Domain Service)
            order.TotalAmount = await _priceCalculator.CalculateAndCreateItemsAsync(order, dto.Items);

            // 4. Calculate Shipping Fee (Domain Service)
            // Truyền TotalAmount để check logic FreeShip > 500k
            order.ShippingFee = _shippingCalculator.CalculateFee(store, address, order.TotalAmount);

            // 5. Finalize & Save
            await FinalizeAndSaveOrderAsync(order);

            // 6. Notify
            await NotifyStaffAsync(
                "Có đơn giao hàng mới! 🛵",
                $"Đơn mới #{order.OrderCode}. Giá trị: {order.GrandTotal:N0}đ",
                order.OrderCode
            );

            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // 2. TẠO ĐƠN TẠI QUẦY (AT COUNTER)
        // =========================================================================
        public async Task<OrderReadDto> CreateAtCounterOrderAsync(int? userId, AtCounterOrderCreateDto dto)
        {
            // 1. Validate
            await ValidateStoreAsync(dto.StoreId);

            if (dto.TableId.HasValue)
            {
                var table = await _unitOfWork.ShopTables.GetByIdAsync(dto.TableId.Value);
                if (table == null || table.StoreId != dto.StoreId)
                    throw new Exception("Bàn không hợp lệ hoặc không thuộc cửa hàng này.");
            }

            // 2. Init Order
            var order = InitializeOrder(dto, userId);

            if (!dto.PaymentMethodId.HasValue)
                throw new AppException("Phương thức thanh toán không hợp lệ.");
            await ValidatePaymentMethodAsync(order, dto.PaymentMethodId.Value);

            // 3. Calculate Items
            order.TotalAmount = await _priceCalculator.CalculateAndCreateItemsAsync(order, dto.Items);

            // 4. Settings (No Ship)
            order.ShippingFee = 0;
            order.DeliveryAddressId = null;

            // 5. Save & Notify
            await FinalizeAndSaveOrderAsync(order);

            await NotifyStaffAsync(
                "Có đơn tại quầy mới! 🍽️",
                $"Đơn mới #{order.OrderCode}. Giá trị: {order.GrandTotal:N0}đ",
                order.OrderCode
            );

            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // 3. TẠO ĐƠN ĐẾN LẤY (PICKUP)
        // =========================================================================
        public async Task<OrderReadDto> CreatePickupOrderAsync(int userId, PickupOrderCreateDto dto)
        {
            // 1. Validate Store
            var store = await ValidateStoreAsync(dto.StoreId);

            // 2. Validate Pickup Time (TimeZone Logic)
            if (dto.PickupTime < DateTime.UtcNow)
                throw new AppException("Thời gian lấy hàng không hợp lệ (phải lớn hơn hiện tại).");

            if (store.OpenTime.HasValue && store.CloseTime.HasValue)
            {
                // Convert UTC to VN Time (UTC+7)
                var pickupTimeVN = dto.PickupTime.ToUniversalTime().AddHours(7);
                var pickupTimeOfDay = pickupTimeVN.TimeOfDay;

                if (pickupTimeOfDay < store.OpenTime.Value || pickupTimeOfDay > store.CloseTime.Value)
                {
                    throw new AppException($"Cửa hàng chỉ mở cửa từ {store.OpenTime.Value:hh\\:mm} đến {store.CloseTime.Value:hh\\:mm}.");
                }
            }

            // 3. Init Order
            var order = InitializeOrder(dto, userId);
            order.OrderType = OrderTypeEnum.Pickup;
            order.PickupCode = GeneratePickupCode();
            order.PickupTime = dto.PickupTime;

            // 4. Validate Payment (Chặn COD cho đơn Pickup)
            var paymentMethod = await ValidatePaymentMethodAsync(order, dto.PaymentMethodId);
            if (paymentMethod.PaymentType == PaymentTypeEnum.COD)
            {
                throw new AppException("Đơn hàng 'Đến lấy' yêu cầu thanh toán trước.");
            }
            order.Status = OrderStatusEnum.PendingPayment; // Luôn Pending vì phải trả trước

            // 5. Process Items
            order.TotalAmount = await _priceCalculator.CalculateAndCreateItemsAsync(order, dto.Items);
            order.ShippingFee = 0;
            order.DeliveryAddressId = null;

            // 6. Save & Notify
            await FinalizeAndSaveOrderAsync(order);

            await NotifyStaffAsync(
                "Có đơn khách đến lấy! 🛍️",
                $"Khách hẹn lấy lúc {order.PickupTime?.AddHours(7):HH:mm}. Mã #{order.OrderCode}.",
                order.OrderCode
            );

            return await GetOrderByIdAsync(order.Id);
        }

        // =========================================================================
        // 4. PRIVATE HELPERS (DRY Principles)
        // =========================================================================

        private async Task<Store> ValidateStoreAsync(int storeId)
        {
            var store = await _unitOfWork.Stores.Find(s => s.Id == storeId)
                                      .Include(s => s.Address)
                                      .FirstOrDefaultAsync();
            if (store == null) throw new KeyNotFoundException("Cửa hàng không tồn tại.");
            if (store.Status != StoreStatusEnum.Active) throw new Exception("Cửa hàng hiện đang đóng cửa.");
            return store;
        }

        private Order InitializeOrder<T>(T dto, int? userId)
        {
            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            order.OrderCode = GenerateOrderCode();
            order.Status = OrderStatusEnum.New;
            return order;
        }

        private async Task<PaymentMethod> ValidatePaymentMethodAsync(Order order, int paymentMethodId)
        {
            var method = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(paymentMethodId)
                         ?? throw new AppException("Phương thức thanh toán không hợp lệ.");

            if (method.PaymentType == PaymentTypeEnum.BankTransfer ||
                method.PaymentType == PaymentTypeEnum.EWallet)
            {
                order.Status = OrderStatusEnum.PendingPayment;
            }
            return method;
        }

        private async Task FinalizeAndSaveOrderAsync(Order order)
        {
            // Tính tổng cuối cùng
            order.DiscountAmount = 0; // TODO: Voucher logic
            order.GrandTotal = order.TotalAmount + (order.ShippingFee ?? 0) - (order.DiscountAmount ?? 0);
            order.CoinsEarned = (int)(order.GrandTotal * 0.01m);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();
        }

        private string GenerateOrderCode() => $"ORD-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        private string GeneratePickupCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task NotifyStaffAsync(string title, string content, string referenceId)
        {
            var staffIds = await _unitOfWork.Repository<User>()
                 .GetAllAsync(u => u.RoleId == UserRoleEnum.Admin ||
                                   u.RoleId == UserRoleEnum.Manager ||
                                   u.RoleId == UserRoleEnum.Staff);

            foreach (var user in staffIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = user.Id,
                    Title = title,
                    Content = content,
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = referenceId
                });
            }
        }

        // =========================================================================
        // 5. CÁC HÀM GET & SUPPORT
        // =========================================================================
        public async Task<OrderReadDto> GetOrderByIdAsync(long id)
        {
            var order = await _unitOfWork.Orders.Find(o => o.Id == id)
                .AsNoTracking() // Optimize Read
                .Include(o => o.Store)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem).ThenInclude(ti => ti.InverseParentItem)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Table)
                .Include(o => o.Shipper)
                .Include(o => o.OrderPayments)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync();

            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
            return _mapper.Map<OrderReadDto>(order);
        }

        public async Task<PagedResult<OrderReadDto>> GetMyOrdersAsync(int userId, PagingRequest request)
        {
            var query = _unitOfWork.Orders.Find(o => o.UserId == userId);
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(o => o.CreatedAt)
                                  .Skip((request.PageIndex - 1) * request.PageSize)
                                  .Take(request.PageSize)
                                  .AsNoTracking()
                                  .Include(o => o.Store)
                                  .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem)
                                  .Include(o => o.OrderPayments)
                                  .ToListAsync();

            var dtos = _mapper.Map<List<OrderReadDto>>(data);
            return new PagedResult<OrderReadDto>(dtos, totalRow, request.PageIndex, request.PageSize);
        }

        public async Task<OrderReadDto> GetOrderByOrderCodeAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders.Find(o => o.OrderCode == orderCode)
                .AsNoTracking()
                .Include(o => o.Store)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.InverseParentItem).ThenInclude(ti => ti.InverseParentItem)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Table)
                .Include(o => o.Shipper)
                .Include(o => o.OrderPayments)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync();

            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
            return _mapper.Map<OrderReadDto>(order);
        }

        // =========================================================================
        // 6. LỌC & THỐNG KÊ
        // =========================================================================
        public async Task<PagedResult<OrderReadDto>> GetOrdersByFilterAsync(OrderFilterDto filter)
        {
            var query = _unitOfWork.Orders.GetQueryable()
                .AsNoTracking()
                .Include(o => o.Store)
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.Table)
                .AsQueryable();

            // Filter logic
            if (filter.IsDeleted) query = query.IgnoreQueryFilters().Where(o => o.DeletedAt != null);
            else query = query.Where(o => o.DeletedAt == null);

            if (filter.StoreId.HasValue) query = query.Where(o => o.StoreId == filter.StoreId);
            if (filter.Status.HasValue) query = query.Where(o => o.Status == filter.Status);
            if (filter.FromDate.HasValue) query = query.Where(o => o.CreatedAt >= filter.FromDate.Value.Date);
            if (filter.ToDate.HasValue) query = query.Where(o => o.CreatedAt < filter.ToDate.Value.Date.AddDays(1));

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                string k = filter.Keyword.Trim().ToLower();
                query = query.Where(o =>
                    o.OrderCode.ToLower().Contains(k) ||
                    (o.RecipientName != null && o.RecipientName.ToLower().Contains(k)) ||
                    (o.RecipientPhone != null && o.RecipientPhone.Contains(k)) ||
                    (o.User != null && o.User.Username.ToLower().Contains(k))
                );
            }
            if (filter.UserPublicId.HasValue)
            {
                query = query.Where(o => o.User != null && o.User.PublicId == filter.UserPublicId);
            }

            int totalRecords = await query.CountAsync();
            var items = await query.OrderByDescending(o => o.CreatedAt)
                                   .Skip((filter.PageIndex - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync();

            var dtos = _mapper.Map<List<OrderReadDto>>(items);
            return new PagedResult<OrderReadDto>(dtos, totalRecords, filter.PageIndex, filter.PageSize);
        }

        public async Task<OrderQuickStatsDto> GetQuickStatsAsync(int? storeId, DateTime? dateInput)
        {
            // TimeZone Handling
            TimeZoneInfo vnTimeZone;
            try { vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); }
            catch { vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); }

            DateTime targetDateVn = dateInput?.Date ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(targetDateVn, vnTimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(targetDateVn.AddDays(1).AddTicks(-1), vnTimeZone);

            var baseQuery = _unitOfWork.Orders.GetQueryable();
            if (storeId.HasValue) baseQuery = baseQuery.Where(o => o.StoreId == storeId);

            var dailyQuery = baseQuery.Where(o => o.CreatedAt >= startUtc && o.CreatedAt <= endUtc);

            return new OrderQuickStatsDto
            {
                TodayRevenue = await dailyQuery
                    .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                    .SumAsync(o => (decimal?)o.GrandTotal) ?? 0,

                TodayOrders = await dailyQuery.CountAsync(),

                TotalRevenueAllTime = await baseQuery
                    .Where(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received)
                    .SumAsync(o => (decimal?)o.GrandTotal) ?? 0,

                TotalCompletedOrders = await baseQuery
                    .CountAsync(o => o.Status == OrderStatusEnum.Completed || o.Status == OrderStatusEnum.Received),

                PendingOrders = await baseQuery
                    .CountAsync(o => o.Status == OrderStatusEnum.New || o.Status == OrderStatusEnum.Confirmed || o.Status == OrderStatusEnum.Preparing),

                ShippingOrders = await baseQuery
                    .CountAsync(o => o.Status == OrderStatusEnum.Delivering)
            };
        }

        // =========================================================================
        // 7. CẬP NHẬT TRẠNG THÁI (CORE LOGIC)
        // =========================================================================
        public async Task<OrderReadDto> UpdateOrderStatusAsync(long orderId, OrderStatusEnum newStatus, UserRoleEnum actorRole)
        {
            var order = await _unitOfWork.Orders.Find(o => o.Id == orderId)
                .Include(o => o.PaymentMethod)
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product) // 🟢 Include Product để update TotalSold
                .FirstOrDefaultAsync()
                ?? throw new AppException("Đơn hàng không tồn tại.");

            var paymentSnapshot = await _orderPaymentService.BuildPaymentSnapshotAsync(orderId);

            // 1. Validate Transition
            OrderStateMachine.ValidateTransition(order.Status, newStatus, paymentSnapshot, actorRole, order.GrandTotal);

            // 2. Logic khi Hoàn tất
            if (newStatus == OrderStatusEnum.Completed || newStatus == OrderStatusEnum.Received)
            {
                // Auto Payment
                if (!paymentSnapshot.IsFullyPaid(order.GrandTotal) && order.PaymentMethod != null)
                {
                    await _orderPaymentService.AutoConfirmPaymentAsync(
                        order.Id,
                        order.PaymentMethod.Id,
                        order.PaymentMethod.Name,
                        order.GrandTotal - paymentSnapshot.PaidAmount,
                        "Hệ thống tự động xác nhận thanh toán."
                    );
                }

                // Update TotalSold
                foreach (var item in order.OrderItems.Where(oi => oi.ParentItemId == null))
                {
                    if (item.Product != null)
                    {
                        item.Product.TotalSold += item.Quantity;
                        _unitOfWork.Repository<Product>().Update(item.Product);
                    }
                }
            }

            // 3. Update & Save
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            // 4. Notify User
            if (order.UserId.HasValue)
            {
                string msg = newStatus switch
                {
                    OrderStatusEnum.Confirmed => $"Đơn hàng #{order.OrderCode} đã được xác nhận 🥤.",
                    OrderStatusEnum.Delivering => $"Shipper đang giao đơn hàng #{order.OrderCode} 🛵.",
                    OrderStatusEnum.Completed => $"Đơn hàng #{order.OrderCode} hoàn thành. Chúc bạn ngon miệng! ⭐",
                    OrderStatusEnum.Received => $"Món đã sẵn sàng! Vui lòng tới quầy nhận đơn #{order.OrderCode}.",
                    OrderStatusEnum.Cancelled => $"Đơn hàng #{order.OrderCode} đã bị hủy.",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(msg))
                {
                    await _notificationService.CreateAsync(new NotificationCreateDto
                    {
                        UserId = order.UserId.Value,
                        Title = "Cập nhật đơn hàng",
                        Content = msg,
                        Type = NotificationTypeEnum.Order,
                        ReferenceId = order.OrderCode
                    });
                }
            }

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<bool> CancelOrderAsync(long orderId, int? userId, OrderCancelDto cancelDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");

            if (order.Status != OrderStatusEnum.New && order.Status != OrderStatusEnum.PendingPayment)
                throw new Exception("Đơn hàng đã được tiếp nhận, không thể hủy.");

            order.Status = OrderStatusEnum.Cancelled;
            order.CancelReason = cancelDto.Reason;
            order.CancelNote = cancelDto.Note;
            order.CancelledByUserId = userId ?? 0;

            // Revert Voucher
            if (order.UserVoucherId.HasValue)
            {
                var uv = await _unitOfWork.Repository<UserVoucher>().GetByIdAsync(order.UserVoucherId.Value);
                if (uv != null)
                {
                    uv.Status = UserVoucherStatusEnum.Unused;
                    uv.UsedDate = null;
                    uv.OrderIdUsed = null;
                    _unitOfWork.Repository<UserVoucher>().Update(uv);
                }
            }

            _unitOfWork.Orders.Update(order);
            var success = await _unitOfWork.CompleteAsync() > 0;

            if (success && order.UserId.HasValue)
            {
                var paymentSnapshot = await _orderPaymentService.BuildPaymentSnapshotAsync(orderId);
                string content = paymentSnapshot.IsFullyPaid(order.GrandTotal)
                    ? $"Đơn hàng #{order.OrderCode} đã hủy. Cửa hàng sẽ hoàn tiền trong 24h."
                    : $"Đơn hàng #{order.OrderCode} đã hủy thành công.";

                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = order.UserId.Value,
                    Title = "Đơn hàng đã bị hủy",
                    Content = content,
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = order.OrderCode
                });
            }
            return success;
        }

        // =========================================================================
        // 8. OTHER ACTIONS
        // =========================================================================
        public async Task<bool> AssignShipperAsync(long orderId, int shipperId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Đơn hàng không tồn tại.");
            if (order.OrderType != OrderTypeEnum.Delivery) throw new AppException("Không thể gán Shipper cho đơn này.");

            var shipper = await _unitOfWork.Repository<User>().GetByIdAsync(shipperId);
            if (shipper == null) throw new Exception("Shipper không hợp lệ.");

            order.ShipperId = shipperId;
            order.Status = OrderStatusEnum.Delivering;
            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> VerifyPickupCodeAsync(long orderId, string code)
        {
            var order = await _unitOfWork.Orders.Find(o => o.Id == orderId && o.PickupCode == code).FirstOrDefaultAsync();
            if (order == null) throw new KeyNotFoundException("Mã lấy đồ không đúng.");
            if (order.Status == OrderStatusEnum.Received || order.Status == OrderStatusEnum.Completed)
                throw new AppException("Đơn hàng này đã được nhận rồi.");

            order.Status = OrderStatusEnum.Received;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<decimal> CalculateShippingFeeAsync(int storeId, long addressId)
        {
            var store = await _unitOfWork.Stores.Find(s => s.Id == storeId).Include(s => s.Address).FirstOrDefaultAsync();
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

            if (store == null || address == null) throw new KeyNotFoundException("Thông tin không hợp lệ.");

            // Dùng 0 cho TotalAmount vì đây chỉ là tính thử
            return _shippingCalculator.CalculateFee(store, address, 0);
        }

        public async Task<bool> SoftDeleteOrderAsync(long id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return false;
            order.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> RestoreOrderAsync(long id)
        {
            var order = await _unitOfWork.Orders.GetQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return false;
            order.DeletedAt = null;
            _unitOfWork.Orders.Update(order);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}