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
            string imageName = String.Empty;
            if (model.Image != null)
            {
                imageName = Guid.NewGuid().ToString() + ".jpg";
                var dir = configuration["ImageDir"];
                var fileSave = Path.Combine(Directory.GetCurrentDirectory(), dir, imageName);
                using (var stream = new FileStream(fileSave, FileMode.Create))
                    await model.Image.CopyToAsync(stream);
            }

            var entity = mapper.Map<ProductEntity>(model);
            entity.Image = imageName;
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return Ok(entity.Id);
        }

        // PUT: api/Product/2
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromForm] ProductEditModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Мапимо модель редагування на сутність
            mapper.Map(model, product);

            // Якщо надіслано новий файл зображення
            if (model.ImageFile != null)
            {
                string imageName = Guid.NewGuid().ToString() + ".jpg";
                var dir = configuration["ImageDir"];
                var fileSave = Path.Combine(Directory.GetCurrentDirectory(), dir, imageName);

                using (var stream = new FileStream(fileSave, FileMode.Create))
                    await model.ImageFile.CopyToAsync(stream);

                // Видаляємо старе зображення, якщо існує
                if (!string.IsNullOrEmpty(product.Image))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), dir, product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                product.Image = imageName;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Product/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Якщо продукт має зображення, видаляємо його з файлової системи
            if (!string.IsNullOrEmpty(product.Image))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["ImageDir"], product.Image);
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
