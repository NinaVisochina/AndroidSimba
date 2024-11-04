using Bogus;
using Microsoft.EntityFrameworkCore;
using WebSimba.Data;
using WebSimba.Data.Entities;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        // Ініціалізація категорій
        if (!_context.Categories.Any())
        {
            var categoryFaker = new Faker<CategoryEntity>("uk")
                .RuleFor(c => c.Name, f => f.Commerce.Department());

            var categories = categoryFaker.Generate(10);
            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        // Ініціалізація продуктів
        if (!_context.Products.Any())
        {
            var productFaker = new Faker<ProductEntity>("uk")
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(p => p.CategoryId, f => _context.Categories
                    .OrderBy(c => Guid.NewGuid())
                    .Select(c => c.Id)
                    .FirstOrDefault())
                .RuleFor(p => p.Image, "noimage.jpg");

            var products = productFaker.Generate(20);
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }
    }
}

