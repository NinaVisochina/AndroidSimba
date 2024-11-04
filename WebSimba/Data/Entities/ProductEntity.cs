using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSimba.Data.Entities
{
    [Table("tblProducts")]
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string? Image { get; set; } // Шлях до зображення продукту

        // Зовнішній ключ до категорії
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // Навігаційна властивість до категорії
        public virtual CategoryEntity? Category { get; set; }
    }
}
