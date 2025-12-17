// File: Interfaces/OptionInterfaces/IEnumOptionService.cs


// File: Interfaces/OptionInterfaces/IEnumOptionService.cs

using drinking_be.Dtos.OptionDtos;

namespace drinking_be.Interfaces.ProductInterfaces
{
    // Interface chung để đọc danh sách các giá trị Enum (Id và Label)
    public interface IEnumOptionService
    {
        // Sử dụng DTO chung để đọc danh sách Enum
        Task<IEnumerable<OptionReadDto>> GetIceLevelsAsync();
        Task<IEnumerable<OptionReadDto>> GetSugarLevelsAsync();
    }
}