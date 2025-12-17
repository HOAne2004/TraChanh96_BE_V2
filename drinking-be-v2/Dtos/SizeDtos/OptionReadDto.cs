// File: Dtos/OptionDtos/OptionReadDto.cs

namespace drinking_be.Dtos.OptionDtos
{
    public class OptionReadDto
    {
        // ID là giá trị số (byte) của Enum
        public byte Id { get; set; }

        // Label là tên hiển thị của Enum (ví dụ: 100% Đá, Ít Đường)
        public string Label { get; set; } = null!;
    }
}