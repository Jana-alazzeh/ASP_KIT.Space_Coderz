using July_Team.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace July_Team.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsApiController(AppDbContext db)
        {
            _db = db;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    ImageUrl_Back = p.ImageUrl_Back
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "المنتج غير موجود" });

            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                ImageUrl_Back = product.ImageUrl_Back
            };

            return Ok(dto);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                ImageUrl_Back = dto.ImageUrl_Back
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return Ok(new { message = "تمت إضافة المنتج بنجاح", id = product.Id });
        }

        // ✅ 4. UPDATE - Edit existing product
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "المنتج غير موجود" });

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.ImageUrl_Back = dto.ImageUrl_Back;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return Ok(new { message = "تم تعديل المنتج بنجاح" });
        }

        // ✅ 5. DELETE - Remove product
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "المنتج غير موجود" });

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return Ok(new { message = "تم حذف المنتج بنجاح" });
        }
    }


}
