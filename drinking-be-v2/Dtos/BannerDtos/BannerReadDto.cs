namespace drinking_be.Dtos.BannerDtos
{
    public class BannerReadDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? LinkUrl { get; set; }
        public int SortOrder { get; set; }
        public string? Position { get; set; }
    }
}