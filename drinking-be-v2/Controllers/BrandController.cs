using drinking_be.Dtos.BrandDtos;
using drinking_be.Dtos.ProductDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IProductStoreProvisionService _productStoreProvisionService;
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(
            IBrandService brandService,
            IProductStoreProvisionService productStoreProvisionService,
            IUnitOfWork unitOfWork)
        {
            _brandService = brandService;
            _productStoreProvisionService = productStoreProvisionService;
            _unitOfWork = unitOfWork;
        }

        // --- HELPER ---
        private BrandReadDto GetDefaultBrandInfo()
        {
            return new BrandReadDto
            {
                Name = "Trà Chanh 96 (Mặc định)",
                CompanyName = "Công ty TNHH Trà Chanh 96",
                LogoUrl = "https://placehold.co/100x100?text=Logo",
                Address = "Vui lòng cập nhật thông tin trong trang quản trị",
                Hotline = "1900 xxxx",
                EmailSupport = "support@trachanh96.vn",
                Status = "Active"
            };
        }

        // --- PUBLIC API (Dùng cho cả Footer, AppConfig, Contact...) ---

        [HttpGet("info")] // Đặt route là /api/brand/info
        [AllowAnonymous]  // Khách không cần login cũng xem được
        public async Task<IActionResult> GetPublicBrandInfo()
        {
            var brand = await _brandService.GetPrimaryBrandInfoAsync();
            // Luôn trả về dữ liệu (Thật hoặc Mặc định) để FE không bị crash
            return Ok(brand ?? GetDefaultBrandInfo());
        }

        // --- ADMIN API (Quản lý) ---

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] BrandCreateDto brandDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _brandService.CreateBrandAsync(brandDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDto brandDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _brandService.UpdateBrandAsync(id, brandDto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPut("{storeId}/products/{productId}/enable")]
        public async Task<IActionResult> Enable(int storeId, int productId)
        {
            int brandId = GetBrandIdFromToken();
            await _productStoreProvisionService.EnableAsync(brandId, storeId, productId);
            return Ok();
        }

        [HttpPut("{storeId}/products/{productId}/disable")]
        public async Task<IActionResult> Disable(int storeId, int productId)
        {
            int brandId = GetBrandIdFromToken();
            await _productStoreProvisionService.DisableAsync(brandId, storeId, productId);
            return Ok();
        }
        private int GetBrandIdFromToken()
        {
            var brandIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "BrandId" || c.Type == "brand_id");

            if (brandIdClaim == null)
                return 1;

            return int.Parse(brandIdClaim.Value);
        }

        [HttpGet("stores/{storeId}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStoreProducts(int storeId)
        {
            int brandId = GetBrandIdFromToken();
            var productStores = await _unitOfWork.Repository<ProductStore>()
                .GetAllAsync(ps => ps.StoreId == storeId);

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p => p.BrandId == brandId && p.Status == ProductStatusEnum.Active);

            // 👇 MAP DỮ LIỆU PHẲNG (FLAT)
            var result = products.Select(p =>
            {
                var ps = productStores.FirstOrDefault(x => x.ProductId == p.Id);
                return new ProductStoreAdminReadDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    BasePrice = p.BasePrice,
                    ImageUrl = p.ImageUrl,
                    Status = ps?.Status ?? ProductStoreStatusEnum.Disabled
                };
            });

            return Ok(result);
        }

    }
}
