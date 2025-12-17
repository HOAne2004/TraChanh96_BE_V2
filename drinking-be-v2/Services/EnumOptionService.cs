// File: Services/EnumOptionService.cs (Đã được sửa để sử dụng OptionReadDto)

using drinking_be.Enums;
using drinking_be.Dtos.OptionDtos;
using System.ComponentModel;
using System.Reflection;
using drinking_be.Interfaces.ProductInterfaces;

namespace drinking_be.Services
{
    public class EnumOptionService : IEnumOptionService
    {
        // Hàm đọc Enum, sử dụng [Description] cho tên hiển thị
        private IEnumerable<OptionReadDto> GetEnumOptions<TEnum>() where TEnum : Enum
        {
            var options = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new OptionReadDto
                {
                    Id = Convert.ToByte(e),
                    Label = e.GetType()
                             .GetMember(e.ToString())
                             .FirstOrDefault()?
                             .GetCustomAttribute<DescriptionAttribute>()?
                             .Description ?? e.ToString() // Lấy Description hoặc tên Enum nếu không có
                })
                .ToList();
            return options;
        }

        public Task<IEnumerable<OptionReadDto>> GetIceLevelsAsync()
        {
            var options = GetEnumOptions<IceLevelEnum>();
            return Task.FromResult(options);
        }

        public Task<IEnumerable<OptionReadDto>> GetSugarLevelsAsync()
        {
            var options = GetEnumOptions<SugarLevelEnum>();
            return Task.FromResult(options);
        }
    }
}