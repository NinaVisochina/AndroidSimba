namespace WebSimba.Models.Category
{
    public class CategoryItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //При читані категорії сервер повідомляє, шлях до фото
        public string ImagePath { get; set; } = string.Empty;
    }
}