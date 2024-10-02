using Microsoft.EntityFrameworkCore;
using WebSimba.Models;
using Microsoft.EntityFrameworkCore;
using WebSimba.Data.Entities;

namespace WebSimba.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
    }
}
