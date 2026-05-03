using drinking_be.Dtos.StoreDtos;

namespace drinking_be.Interfaces.StoreInterfaces
{
    public interface IStoreAnalyticsService
    {
        Task<StoreDashboardDto> GetStoreDashboardAsync(int storeId, string timeFilter);
    }
}