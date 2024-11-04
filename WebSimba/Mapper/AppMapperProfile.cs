using AutoMapper;
using WebSimba.Data.Entities;
using WebSimba.Models.Category;
using WebSimba.Models.Product;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebSimba.Mapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {
            //Початок
            CreateMap<CategoryEntity, CategoryItemModel>()
                .ForMember(x => x.ImagePath, opt => opt.MapFrom(x =>
                    string.IsNullOrEmpty(x.Image) ? "/images/noimage.jpg" : $"/images/{x.Image}"));

            CreateMap<CategoryCreateModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore());

            CreateMap<CategoryEditModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Condition((src, dest, srcMember) => src.ImageFile != null));

            // Мапінг для продуктів
            CreateMap<ProductEntity, ProductItemModel>()
                .ForMember(x => x.ImagePath, opt => opt.MapFrom(x =>
                    string.IsNullOrEmpty(x.Image) ? "/images/noimage.jpg" : $"/images/{x.Image}"))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : string.Empty));

            CreateMap<ProductCreateModel, ProductEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore()); // Зображення завантажується окремо

            CreateMap<ProductEditModel, ProductEntity>()
                .ForMember(x => x.Image, opt => opt.Condition((src, dest, srcMember) => src.ImageFile != null));

        }
    }
}


