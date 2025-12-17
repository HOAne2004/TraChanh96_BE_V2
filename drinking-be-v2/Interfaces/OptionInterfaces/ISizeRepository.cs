using drinking_be.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace drinking_be.Interfaces.OptionInterfaces
{
    // Kế thừa IGenericRepository và bổ sung phương thức cần thiết cho OrderService
    public interface ISizeRepository : IGenericRepository<Size>
    {
        // Phương thức để lấy nhiều size theo ID
        //Task<IEnumerable<Size>> GetSizesByIdsAsync(List<short> sizeIds);
    }
}