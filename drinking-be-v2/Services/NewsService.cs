using AutoMapper;
using drinking_be.Dtos.NewsDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Models;
using drinking_be.Utils; // Cần class SlugGenerator

namespace drinking_be.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // --- PUBLIC METHODS ---

        public async Task<IEnumerable<NewsReadDto>> GetPublishedNewsAsync()
        {
            var repo = _unitOfWork.Repository<News>();

            // Lấy tin Active (Published) và sắp xếp mới nhất
            var news = await repo.GetAllAsync(
                filter: n => n.Status == ContentStatusEnum.Published,
                orderBy: q => q.OrderByDescending(n => n.PublishedDate ?? n.CreatedAt),
                includeProperties: "User" // Lấy thông tin người viết
            );

            return _mapper.Map<IEnumerable<NewsReadDto>>(news);
        }

        public async Task<NewsReadDto?> GetNewsBySlugAsync(string slug)
        {
            var repo = _unitOfWork.Repository<News>();

            var news = await repo.GetFirstOrDefaultAsync(
                filter: n => n.Slug == slug && n.Status == ContentStatusEnum.Published,
                includeProperties: "User,Comments,Comments.User" // Include cả Comment nếu cần
            );

            return news == null ? null : _mapper.Map<NewsReadDto>(news);
        }

        // --- ADMIN METHODS ---

        public async Task<IEnumerable<NewsReadDto>> GetAllNewsAsync(string? search, ContentStatusEnum? status)
        {
            var repo = _unitOfWork.Repository<News>();

            var query = await repo.GetAllAsync(
                includeProperties: "User",
                orderBy: q => q.OrderByDescending(n => n.CreatedAt)
            );

            // 1. Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(n => n.Status == status.Value);
            }

            // 2. Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(search));
            }

            return _mapper.Map<IEnumerable<NewsReadDto>>(query);
        }

        public async Task<NewsReadDto?> GetNewsByIdAsync(int id)
        {
            var news = await _unitOfWork.Repository<News>().GetFirstOrDefaultAsync(
                filter: n => n.Id == id,
                includeProperties: "User"
            );
            return news == null ? null : _mapper.Map<NewsReadDto>(news);
        }

        public async Task<NewsReadDto> CreateNewsAsync(int userId, NewsCreateDto dto)
        {
            var repo = _unitOfWork.Repository<News>();

            var news = _mapper.Map<News>(dto);
            news.UserId = userId;
            news.PublicId = Guid.NewGuid();
            news.CreatedAt = DateTime.UtcNow;

            // Tự động tạo Slug
            news.Slug = SlugGenerator.GenerateSlug(news.Title);

            // Nếu tạo là Published luôn -> Gán ngày Published
            if (news.Status == ContentStatusEnum.Published)
            {
                news.PublishedDate = DateTime.UtcNow;
            }

            await repo.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();

            return (await GetNewsByIdAsync(news.Id))!;
        }

        public async Task<NewsReadDto?> UpdateNewsAsync(int id, NewsUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<News>();
            var news = await repo.GetByIdAsync(id);

            if (news == null) return null;

            // Logic cập nhật Slug nếu Title thay đổi (Tùy chọn)
            // if (!string.IsNullOrEmpty(dto.Title) && dto.Title != news.Title) {
            //     news.Slug = SlugGenerator.GenerateSlug(dto.Title);
            // }
            // Hoặc cho phép Admin tự nhập Slug trong DTO (như code hiện tại)

            _mapper.Map(dto, news);
            news.UpdatedAt = DateTime.UtcNow;

            // Nếu chuyển trạng thái sang Published -> Cập nhật ngày
            if (dto.Status == ContentStatusEnum.Published && news.PublishedDate == null)
            {
                news.PublishedDate = DateTime.UtcNow;
            }

            repo.Update(news);
            await _unitOfWork.SaveChangesAsync();

            return (await GetNewsByIdAsync(id))!;
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var repo = _unitOfWork.Repository<News>();
            var news = await repo.GetByIdAsync(id);

            if (news == null) return false;

            // Soft Delete
            news.Status = ContentStatusEnum.Deleted;
            news.DeletedAt = DateTime.UtcNow;

            repo.Update(news);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}