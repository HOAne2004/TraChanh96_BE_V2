using drinking_be.Dtos.ProductDtos;
using drinking_be.Interfaces.ProductInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drinking_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products?search=tea&categorySlug=tra-sua&sort=price_asc
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? categorySlug, [FromQuery] string? sort)
        {
            var products = await _productService.GetAllAsync(search, categorySlug, sort);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _productService.GetBySlugAsync(slug);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products (Chỉ Admin/Manager)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newProduct = await _productService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto updateDto)
        {
            var updatedProduct = await _productService.UpdateAsync(id, updateDto);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: api/products/store/{storeId}
        // Lấy menu cho khách hàng tại một cửa hàng cụ thể
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetMenuByStore(int storeId, [FromQuery] string? search, [FromQuery] string? categorySlug)
        {
            var menu = await _productService.GetMenuByStoreAsync(storeId, search, categorySlug);
            return Ok(menu);
        }

        // PATCH: api/products/store-status
        // Dành cho Store Manager cập nhật trạng thái món (Hết hàng/Có hàng)
        [HttpPatch("store-status")]
        [Authorize(Roles = "Admin,Manager,StoreManager")] // Bổ sung Role StoreManager nếu có
        public async Task<IActionResult> UpdateStoreStatus([FromBody] ProductStoreUpdateDto updateDto)
        {
            // TODO: Nếu kỹ tính, check xem User hiện tại có phải quản lý StoreId này không
            var result = await _productService.UpdateProductStatusAtStoreAsync(updateDto);

            if (!result) return NotFound("Không tìm thấy sản phẩm tại cửa hàng này.");

            return NoContent();
        }
    }
}