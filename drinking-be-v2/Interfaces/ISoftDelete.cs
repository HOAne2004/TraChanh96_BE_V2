using System;

namespace drinking_be.Interfaces
{
    // Bất kỳ bảng nào muốn được dọn dẹp tự động thì phải kế thừa Interface này
    public interface ISoftDelete
    {
        DateTime? DeletedAt { get; set; }
    }
}