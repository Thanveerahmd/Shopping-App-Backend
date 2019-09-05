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

            CreateMap<PhotoForUser, PhotoDto>();

            CreateMap<PhotoForUser, PhotoForReturnDto>();

            CreateMap<PhotoUploadDto, PhotoForUser>();

            CreateMap<PhotoUploadDto, PhotoForAd>();

            CreateMap<PhotoUploadDto, PhotoForCategory>();

            CreateMap<Cart, CartDto>();

            CreateMap<CartDto, Cart>();

            CreateMap<DeliveryInfo, DeliveryInfoDto>();

            CreateMap<CartProduct, CartProductDto>();

            CreateMap<CartProduct, CartProductToReturn>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoUploadDto, Photo>();

            CreateMap<Rating, RatingDto>();

            CreateMap<RatingDto, Rating>();

            CreateMap<BillingInfo, DeliveryInfoDto>();

            CreateMap<DeliveryInfoDto, BillingInfo>();

            CreateMap<DeliveryInfoDto, DeliveryInfo>();

            CreateMap<DeliveryInfo, DeliveryInfoDto>();

            CreateMap<BillingInfo, BillingInfoDto>();

            CreateMap<BillingInfoDto, BillingInfo>();

            CreateMap<StoreDto, Store>();

            CreateMap<Store, StoreDto>();

            CreateMap<OrderProductDto, orderDetails>();

            CreateMap<orderDetails, OrderProductDto>();

            CreateMap<CartProduct, OrderProductDto>();

            CreateMap<orderDetails, checkoutDto>();

            CreateMap<checkoutDto, orderDetails>();

            CreateMap<AdvertismentUploadDto, Advertisement>();

            CreateMap<Advertisement, AdvertismentUploadDto>();

            CreateMap<ProductAddingDto, Product>();

            CreateMap<Product, ProductAddingDto>();

            CreateMap<Advertisement, AdvertisementToReturnDto>();

            CreateMap<AdvertisementToReturnDto, Advertisement>();

            CreateMap<PaymentInfoDto, SellerPaymentInfo>();

            CreateMap<SellerPaymentInfo, PaymentInfoDto>();

            CreateMap<PaymentInfoDto, BuyerPaymentInfo>();

            CreateMap<BuyerPaymentInfo, PaymentInfoDto>();

            CreateMap<DeviceDetailsDto, DeviceToken>();

            CreateMap<OrderProductDto, Order>();

            CreateMap<Order, OrderProductDto>();

            CreateMap<orderDetails, OrderInfoForSellerDto>();

            CreateMap<ChatDto, Chat>();

            CreateMap<Chat, ChatDto>();

            CreateMap<PromoDto, Promo>();

            CreateMap<Promo, PromoDto>();

            CreateMap<PromoStatusUpdateDto, Promo>();

            CreateMap<Promo, PromoStatusUpdateDto>();

            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryDto, Category>();

            CreateMap<Category, CategoryReturnDto>();

            CreateMap<CategoryReturnDto, Category>();

            CreateMap<SubCategory, SubCategoryDto>();

            CreateMap<SubCategoryDto, SubCategory>();

            CreateMap<OrderDetailsForUserPreference, orderDetails>();
            
            CreateMap<AdvertScoreDto,Advertisement>();

            CreateMap<Advertisement,AdvertScoreDto>();

            CreateMap<Promo,PromoScoreDto>();

            CreateMap<PromoScoreDto,Promo>();

            CreateMap< orderDetails,OrderDetailsForUserPreference>();
        }
    }
}