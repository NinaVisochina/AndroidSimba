using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using WebSimba.Constants;
using WebSimba.Data.Entities.Identity;
using WebSimba.Data.Entities;
using WebSimba.Interfaces;

namespace WebSimba.Data
{
    public static class SeederDB
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var imageWorker = scope.ServiceProvider.GetRequiredService<IImageWorker>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();


                dbContext.Database.Migrate(); //Запусти міграції на БД, якщо їх там немає
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                if (!dbContext.Categories.Any())
                {
                    const int number = 10;
                    var categories = new Faker("uk").Commerce
                        .Categories(number);
                    foreach (var name in categories)
                    {
                        var entity = dbContext.Categories.SingleOrDefault(c => c.Name == name);
                        if (entity != null)
                            continue;

                        string image = imageWorker.Save("https://picsum.photos/1200/800?category").Result;
                        entity = new CategoryEntity
                        {
                            Name = name,
                            Image = image
                        };
                        dbContext.Categories.Add(entity);
                        dbContext.SaveChanges();
                    }
                }

                if (!dbContext.Products.Any())
                {
                    var catIds = dbContext.Categories.Select(x => x.Id).ToArray();
                    const int productNumbers = 30;
                    var faker = new Faker();
                    for (int i = 0; i < productNumbers; i++)
                    {

                        var product = new ProductEntity
                        {
                            Name = faker.Commerce.ProductName(),
                            CategoryId = faker.PickRandom(catIds),
                            Price = Decimal.Parse(faker.Commerce.Price(100, 3000))
                        };
                        int imageCount = faker.Random.Number(3, 6);
                        for (int j = 0; j < imageCount; j++)
                        {
                            string image = imageWorker.Save("https://picsum.photos/1200/800?product").Result;
                            ProductImageEntity pi = new ProductImageEntity
                            {
                                Priority = j,
                                Image = image,
                                Product = product
                            };
                            dbContext.ProductImages.Add(pi);
                        }
                        dbContext.Products.Add(product);
                        dbContext.SaveChanges();
                    }
                }

                if (!dbContext.Roles.Any())
                {
                    foreach (var role in Roles.GetAll)
                    {
                        var result = roleManager.CreateAsync(new RoleEntity { Name = role }).Result;
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"--Error create Role {role}--");
                        }
                    }
                }

                if (!dbContext.Users.Any())
                {
                    string image = imageWorker.Save("https://picsum.photos/1200/800?person").Result;
                    var user = new UserEntity
                    {
                        Email = "admin@gmail.com",
                        UserName = "admin@gmail.com",
                        LastName = "Підкаблучник",
                        FirstName = "Іван",
                        Image = image
                    };
                    var result = userManager.CreateAsync(user, "123456").Result;
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"--Problem create user--{user.Email}");
                    }
                    else
                    {
                        result = userManager.AddToRoleAsync(user, Roles.Admin).Result;
                    }
                }

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("-----------------Seed Conpleted------------- " + elapsedTime);
            }
        }
    }
}