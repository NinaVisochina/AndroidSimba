namespace WebSimba.Interfaces
{
    public interface IImageWorker
    {
        Task<string> Save(IFormFile image);
        Task<string> Save(string urlImage);
        bool Delete(string fileName);
    }
}
