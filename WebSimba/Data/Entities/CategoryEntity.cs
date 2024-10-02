using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSimba.Data.Entities
{
    [Table("tblCategories")]
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Image { get; set; }

        // Зв'язок один до багатьох з продуктами
        public virtual List<ProductEntity>? Products { get; set; }
    }
}