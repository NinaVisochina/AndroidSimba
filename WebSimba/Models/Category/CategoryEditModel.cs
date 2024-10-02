namespace WebSimba.Models.Category
{
    public class CategoryEditModel
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
    }
}
