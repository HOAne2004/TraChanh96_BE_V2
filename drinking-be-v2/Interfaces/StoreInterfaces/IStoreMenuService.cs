using drinking_be.Dtos.ProductDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IStoreMenuService
    {
        Task<IEnumerable<StoreMenuReadDto>> GetMenuByStoreIdAsync(int storeId);
    }
}