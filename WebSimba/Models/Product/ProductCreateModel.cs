namespace WebSimba.Models.Product
{
    public class ProductCreateModel
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public List<IFormFile>? Images { get; set; }

        public int Priority { get; set; }
    }
}
