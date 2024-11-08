using AutoMapper;
using WebSimba.Data.Entities;
using WebSimba.Models.Product;

namespace WebSimba.Mapper
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            // Мапінг ProductEntity на ProductItemModel
            CreateMap<ProductEntity, ProductItemModel>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(img => $"/images/{img.Image}").ToList() : new List<string>()))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Мапінг ProductCreateModel на ProductEntity
            CreateMap<ProductCreateModel, ProductEntity>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Зображення обробляються окремо

            // Мапінг ProductEditModel на ProductEntity
            CreateMap<ProductEditModel, ProductEntity>()
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Оновлення зображень відбувається окремо
        }
    }
}

