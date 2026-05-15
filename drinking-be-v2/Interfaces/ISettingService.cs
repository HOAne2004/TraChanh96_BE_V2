namespace drinking_be.Interfaces
{
    public interface ISettingService
    {
        Task<int> GetIntValueAsync(string key, int defaultValue = 0);
        Task UpdateSettingAsync(string key, string newValue);
    }
}
