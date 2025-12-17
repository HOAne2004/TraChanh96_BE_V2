using drinking_be.Dtos.ShopTableDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IShopTableService
    {
        // Lấy danh sách bàn theo Store/Room
        Task<IEnumerable<ShopTableReadDto>> GetTablesByStoreAsync(int storeId, int? roomId);

        // Lấy chi tiết (Kèm thông tin bàn con nếu là bàn mẹ)
        Task<ShopTableReadDto?> GetTableByIdAsync(int id);

        // Tạo bàn mới
        Task<ShopTableReadDto> CreateTableAsync(ShopTableCreateDto dto);

        // Cập nhật (Đổi tên, Gộp bàn)
        Task<ShopTableReadDto?> UpdateTableAsync(int id, ShopTableUpdateDto dto);

        // Xóa bàn
        Task<bool> DeleteTableAsync(int id);
    }
}