namespace WebSimba.Models.Category
{
    public class CategoryCreateModel
    {
        //Назва категорії
        public string Name { get; set; } = string.Empty;
        //Файл із фото
        public IFormFile? Image { get; set; }
    }
}
