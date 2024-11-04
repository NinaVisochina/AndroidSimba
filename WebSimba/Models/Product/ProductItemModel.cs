namespace WebSimba.Models.Product
{
    public class ProductItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Шлях до зображення продукту, який сервер повідомляє при читанні
        public string ImagePath { get; set; } = string.Empty;

        // Інформація про категорію
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
