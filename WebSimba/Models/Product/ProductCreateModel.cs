namespace WebSimba.Models.Product
{
    public class ProductCreateModel
    {
        // Назва продукту
        public string Name { get; set; } = string.Empty;

        // Ціна продукту
        public decimal Price { get; set; }

        // Ідентифікатор категорії, до якої належить продукт
        public int CategoryId { get; set; }

        // Файл із зображенням продукту
        public IFormFile? Image { get; set; }
    }
}

