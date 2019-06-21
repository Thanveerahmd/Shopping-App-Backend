using System.Linq;
using AutoMapper;
using pro.backend.Dtos;
using pro.backend.Entities;
using Project.Dtos;
using Project.Entities;



namespace Project.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>();

            CreateMap<Admin, AdminDto>();

            CreateMap<AdminDto, Admin>();

            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.isMain).Url);
            });

            CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.isMain).Url);
            });

            CreateMap<Photo, PhotoDto>();

            CreateMap<Cart, CartDto>();

            CreateMap<DeliveryInfo, DeliveryInfoDto>();

            CreateMap<CartProduct, CartProductDto>();

            CreateMap<CartProduct, CartProductToReturn>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoUploadDto, Photo>();

        }
    }
}