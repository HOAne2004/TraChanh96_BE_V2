using AutoMapper;
using drinking_be.Dtos.SupplyOrderDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services
{
    public class SupplyOrderService : ISupplyOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplyOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SupplyOrderReadDto> CreateAsync(int userId, SupplyOrderCreateDto dto)
        {
            var supplyRepo = _unitOfWork.Repository<SupplyOrder>();
            var materialRepo = _unitOfWork.Repository<Material>();

            // 1. Map cơ bản
            var order = new SupplyOrder
            {
                PublicId = Guid.NewGuid(),
                StoreId = dto.StoreId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = SupplyOrderStatusEnum.Pending,
                Note = dto.Note,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                // Tạo mã phiếu: SO-YYYYMMDD-XXXX
                OrderCode = $"SO-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}"
            };

            decimal totalAmount = 0;

            // 2. Xử lý chi tiết (Snapshot giá & đơn vị)
            foreach (var itemDto in dto.Items)
            {
                var material = await materialRepo.GetByIdAsync(itemDto.MaterialId);
                if (material == null) throw new Exception($"Nguyên liệu ID {itemDto.MaterialId} không tồn tại.");

                var orderItem = new SupplyOrderItem
                {
                    MaterialId = itemDto.MaterialId,
                    Quantity = itemDto.Quantity,

                    // Snapshot dữ liệu tại thời điểm nhập
                    Unit = (material.PurchaseUnit ?? material.BaseUnit).ToString(), // Ưu tiên đơn vị nhập
                    CostPerUnit = material.CostPerPurchaseUnit,

                    // Tính thành tiền
                    TotalCost = itemDto.Quantity * material.CostPerPurchaseUnit
                };

                totalAmount += orderItem.TotalCost;
                order.SupplyOrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            // 3. Lưu
            await supplyRepo.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(order.Id))!;
        }

        public async Task<IEnumerable<SupplyOrderReadDto>> GetAllAsync(int? storeId, SupplyOrderStatusEnum? status, DateTime? fromDate, DateTime? toDate)
        {
            var repo = _unitOfWork.Repository<SupplyOrder>();

            var query = await repo.GetAllAsync(
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                includeProperties: "Store,CreatedBy,ApprovedBy"
            );

            if (storeId.HasValue) query = query.Where(o => o.StoreId == storeId.Value);
            if (status.HasValue) query = query.Where(o => o.Status == status.Value);
            if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate.Value);

            return _mapper.Map<IEnumerable<SupplyOrderReadDto>>(query);
        }

        public async Task<SupplyOrderReadDto?> GetByIdAsync(long id)
        {
            var repo = _unitOfWork.Repository<SupplyOrder>();

            // Include chi tiết Items và Material để hiển thị
            var order = await repo.GetFirstOrDefaultAsync(
                filter: o => o.Id == id,
                includeProperties: "Store,CreatedBy,ApprovedBy,SupplyOrderItems,SupplyOrderItems.Material"
            );

            return order == null ? null : _mapper.Map<SupplyOrderReadDto>(order);
        }

        public async Task<SupplyOrderReadDto?> UpdateAsync(long id, int userId, SupplyOrderUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<SupplyOrder>();

            // Lấy Order kèm Items để xử lý nhập kho nếu cần
            var order = await repo.GetFirstOrDefaultAsync(
                filter: o => o.Id == id,
                includeProperties: "SupplyOrderItems,SupplyOrderItems.Material"
            );

            if (order == null) return null;

            // Logic thay đổi trạng thái
            if (dto.Status.HasValue && dto.Status != order.Status)
            {
                // Nếu chuyển sang Approved -> Gán người duyệt
                if (dto.Status == SupplyOrderStatusEnum.Approved && order.Status == SupplyOrderStatusEnum.Pending)
                {
                    order.ApprovedByUserId = userId;
                }

                // ⭐ QUAN TRỌNG: Nếu chuyển sang Received -> Cộng tồn kho
                if (dto.Status == SupplyOrderStatusEnum.Received && order.Status != SupplyOrderStatusEnum.Received)
                {
                    await ProcessInventoryUpdateAsync(order);
                    order.ReceivedAt = dto.ReceivedAt ?? DateTime.UtcNow;
                }

                order.Status = dto.Status.Value;
            }

            // Cập nhật các thông tin khác
            if (!string.IsNullOrEmpty(dto.Note)) order.Note = dto.Note;
            if (dto.ExpectedDeliveryDate.HasValue) order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;

            order.UpdatedAt = DateTime.UtcNow;

            repo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id));
        }

        // --- Helper: Cộng tồn kho ---
        private async Task ProcessInventoryUpdateAsync(SupplyOrder order)
        {
            var inventoryRepo = _unitOfWork.Repository<Inventory>();

            foreach (var item in order.SupplyOrderItems)
            {
                // Tìm xem kho đã có nguyên liệu này chưa
                // Lưu ý: order.StoreId có thể null (Kho tổng) -> Inventory.StoreId cũng null
                var inventory = await inventoryRepo.GetFirstOrDefaultAsync(
                    i => i.MaterialId == item.MaterialId && i.StoreId == order.StoreId
                );

                // Tính toán số lượng quy đổi
                // Ví dụ: Nhập 10 Thùng, ConversionRate = 12 (12 Hộp/Thùng)
                // => Cộng thêm: 10 * 12 = 120 Hộp
                int quantityToAdd = item.Quantity * item.Material.ConversionRate;

                if (inventory == null)
                {
                    // Nếu chưa có thì tạo mới
                    inventory = new Inventory
                    {
                        MaterialId = item.MaterialId,
                        StoreId = order.StoreId,
                        Quantity = quantityToAdd,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await inventoryRepo.AddAsync(inventory);
                }
                else
                {
                    // Nếu có rồi thì cộng dồn
                    inventory.Quantity += quantityToAdd;
                    inventory.UpdatedAt = DateTime.UtcNow;
                    inventoryRepo.Update(inventory);
                }
            }
        }
    }
}