using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebSimba.Data;
using WebSimba.Data.Entities;
using WebSimba.Mapper;

var builder = WebApplication.CreateBuilder(args);

// ������������ PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(AppMapperProfile));

builder.Services.AddControllers();

// ������������ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(opt =>
    opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// ������������ Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

// ������������ ��������� ����� ��� ���������
var dir = builder.Configuration["ImageDir"];
Console.WriteLine("-------Image dir {0}-------", dir);
var dirPath = Path.Combine(Directory.GetCurrentDirectory(), dir);
if (!Directory.Exists(dirPath))
    Directory.CreateDirectory(dirPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dirPath),
    RequestPath = "/images"
});

// ������������ ���������� "noimage.jpg" �� �������������, ���� ���� ����
var imageNo = Path.Combine(dirPath, "noimage.jpg");
if (!File.Exists(imageNo))
{
    string url = "https://m.media-amazon.com/images/I/71QaVHD-ZDL.jpg";
    try
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                File.WriteAllBytes(imageNo, imageBytes);
            }
            else
            {
                Console.WriteLine($"------Failed to retrieve image. Status code: {response.StatusCode}---------");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"-----An error occurred: {ex.Message}------");
    }
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ����������� ���� ����� �� ��������� Seeder
    var seeder = new DatabaseSeeder(dbContext);
    seeder.Seed();
}

// ����������� ���� �����
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbContext.Database.Migrate(); // ����������� ��������� �������

    // ����������� ��������
    if (!dbContext.Categories.Any())
    {
        const int number = 10;
        var categories = new Faker("uk").Commerce.Categories(number);
        foreach (var name in categories)
        {
            var entity = dbContext.Categories.SingleOrDefault(c => c.Name == name);
            if (entity != null)
                continue;

            entity = new CategoryEntity
            {
                Name = name
            };
            dbContext.Categories.Add(entity);
            dbContext.SaveChanges();
        }
    }

    //// ����������� ��������
    //if (!dbContext.Products.Any())
    //{
    //    var productFaker = new Faker<ProductEntity>("uk")
    //        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
    //        .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
    //        .RuleFor(p => p.CategoryId, f => dbContext.Categories
    //            .OrderBy(c => Guid.NewGuid())
    //            .Select(c => c.Id)
    //            .FirstOrDefault())
    //        .RuleFor(p => p.Image, f => "noimage.jpg"); // ���������� ���������� �� �������������

    //    var products = productFaker.Generate(20);
    //    dbContext.Products.AddRange(products);
    //    dbContext.SaveChanges();
    //}
}

app.Run();
