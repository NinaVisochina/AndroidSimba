using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebSimba.Data;
using WebSimba.Data.Entities;
using WebSimba.Models.Product;

namespace WebSimba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ApplicationDbContext _context, IMapper mapper, IConfiguration configuration) : ControllerBase
    {
        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var model = await _context.Products
                .ProjectTo<ProductItemModel>(mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(model);
        }

        // GET: api/Product/2
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductEntity>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] ProductCreateModel model)
        {
            var entity = mapper.Map<ProductEntity>(model);
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            if (model.Images != null && model.Images.Any())
            {
                foreach (var image in model.Images)
                {
                    string imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var dir = configuration["ImageDir"];
                    var fileSave = Path.Combine(Directory.GetCurrentDirectory(), dir, imageName);

                    using (var stream = new FileStream(fileSave, FileMode.Create))
                        await image.CopyToAsync(stream);

                    var productImage = new ProductImageEntity
                    {
                        ProductId = entity.Id,
                        Image = imageName
                    };
                    _context.ProductImages.Add(productImage);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(entity.Id);
        }


        // PUT: api/Product/2
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromForm] ProductEditModel model)
        {
            var product = await _context.Products
                .Include(p => p.Images) // Завантажуємо всі зображення продукту
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Мапимо основні поля з моделі редагування на сутність продукту
            mapper.Map(model, product);

            var dir = configuration["ImageDir"];
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), dir);

            // Додаємо нові зображення, якщо вони надані
            if (model.Images != null && model.Images.Any())
            {
                foreach (var imageFile in model.Images)
                {
                    string imageName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var fileSave = Path.Combine(dirPath, imageName);

                    using (var stream = new FileStream(fileSave, FileMode.Create))
                        await imageFile.CopyToAsync(stream);

                    var productImage = new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Image = imageName,
                        Priority = model.Priority // Можна передавати пріоритет для зображень
                    };
                    _context.ProductImages.Add(productImage);
                }
            }

            // Видаляємо старі зображення, якщо їх потрібно замінити
            if (model.RemoveImageIds != null && model.RemoveImageIds.Any())
            {
                var imagesToRemove = product.Images
                    .Where(img => model.RemoveImageIds.Contains(img.Id))
                    .ToList();

                foreach (var image in imagesToRemove)
                {
                    var imagePath = Path.Combine(dirPath, image.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    _context.ProductImages.Remove(image);
                }
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Product/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images) // Завантажуємо пов'язані зображення
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var dir = configuration["ImageDir"];
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), dir);

            // Видаляємо всі зображення продукту з файлової системи
            foreach (var image in product.Images)
            {
                var imagePath = Path.Combine(dirPath, image.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting image: {ex.Message}");
                    }
                }
            }

            // Видаляємо продукт і всі пов'язані з ним зображення
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
