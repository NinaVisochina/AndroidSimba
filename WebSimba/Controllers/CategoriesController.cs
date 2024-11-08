using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebSimba.Data;
using WebSimba.Data.Entities;
using WebSimba.Models.Category;

namespace WebSimba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ApplicationDbContext _context,
        IMapper mapper, IConfiguration configuration) : ControllerBase
    {
        //private readonly ApplicationDbContext _context;

        //public CategoryController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var model = await _context.Categories
                .ProjectTo<CategoryItemModel>(mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(model);
        }

        // GET: api/Category/2
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryEntity>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromForm] CategoryCreateModel model)
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
            var entity = mapper.Map<CategoryEntity>(model);
            entity.Image = imageName;
            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();
            return Ok(entity.Id);
        }

        // PUT: api/Category/2
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryEntity category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/Category/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}