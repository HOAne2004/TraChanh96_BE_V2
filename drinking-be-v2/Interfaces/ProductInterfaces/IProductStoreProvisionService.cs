using drinking_be.Dtos.ProductDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    public interface IProductStoreProvisionService
    {
        Task InitializeProductStoresForNewStoreAsync(Store store);
        Task EnableAsync(int brandId, int storeId, int productId);
        Task DisableAsync(int brandId, int storeId, int productId);
        Task<IEnumerable<ProductStoreAdminReadDto>> GetProductsByStoreAsync(int brandId, int storeId);

    }
}
