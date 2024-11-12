using Application.DTOs.AuthsDto;
using Application.DTOs.CardDto;
using Application.DTOs.CategoriesDto;
using Application.DTOs.OrdersDto;
using Application.DTOs.ProductsDto;
using Application.DTOs.ReviewDto;
using Application.DTOs.RolesDto;
using Application.DTOs.SuppliersDto;
using Application.DTOs.TransactionDto;
using Application.DTOs.UsersDto;
using Application.DTOs.WishlistDto;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UpdateUserRequest>().ReverseMap();
            CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.Images, opt => opt
             .MapFrom(src => src.Images.Select(img => new ProductImageDto
             {
                 ImageUrl = img.ImageUrl,
                 IsPrimary = img.IsPrimary
             })))
             .ReverseMap();
            CreateMap<CreateUpdateProductDto, Product>()
               .ForMember(dest => dest.Images, opt => opt.Ignore())
               .ReverseMap()
               .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<Product, ProductInListDto>()
             .ForMember(dest => dest.Images, opt => opt
             .MapFrom(src => src.Images.Select(img => new ProductImageDto
             {
                 ImageUrl = img.ImageUrl,
                 IsPrimary = img.IsPrimary
             })))
             .ReverseMap();
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateUpdateCategoryDto, Category>().ReverseMap();
            CreateMap<Category, CategoryInListDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
            CreateMap<CreateUpdateSupplierDto, Supplier>().ReverseMap();
            CreateMap<Supplier, SupplierInListDto>().ReverseMap();
            CreateMap<CreateUpdateOrderDto, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => new List<OrderDetail>())); // Khởi tạo OrderDetails rỗng
            CreateMap<OrderDetailDto, OrderDetail>();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString())).ReverseMap()
                .ForMember(dest => dest.ShippingProvider, opt => opt.MapFrom(src => src.ShippingProvider.ToString())).ReverseMap()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString())).ReverseMap()
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString())).ReverseMap()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<SignInDto, User>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<UserInformatioResponeDto, User>().ReverseMap();
            CreateMap<CreateUpdateUserDto, User>().ReverseMap();
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items, opt => opt
                        .MapFrom(src => src.Items.Select(c => new CartItemsDto()
                        {
                            CartId = c.CartId,
                            ProductId = c.ProductId,
                            ProductName = c.ProductName,
                            Quantity = c.Quantity,
                            UnitPrice = c.UnitPrice
                        }))
                    ).ReverseMap();
            CreateMap<CartItem, CartItemsDto>().ReverseMap();
            CreateMap<PaymentTransaction, TransactionDto>()
                 .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                 .ReverseMap()
                 .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => Enum.Parse(typeof(PaymentMethod), src.PaymentMethod)));
            CreateMap<CreateUpdateTransactionDto, PaymentTransaction>().ReverseMap();
            CreateMap<Review, ReviewDto>()
              .ForMember(dest => dest.ReviewImages, opt => opt
            .MapFrom(src => src.ReviewImages.Select(img => new ReviewImageDto
            {
                ImageUrl = img.ImageUrl,
            })))
            .ReverseMap();
            CreateMap<CreateUpdateReviewDto, Review>()
               .ForMember(dest => dest.ReviewImages, opt => opt.Ignore())
               .ReverseMap()
               .ForMember(dest => dest.ReviewImages, opt => opt.Ignore());
            CreateMap<Review, ReviewInListDto>()
             .ForMember(dest => dest.ReviewImages, opt => opt
             .MapFrom(src => src.ReviewImages.Select(img => new ReviewImageDto
             {
                 ImageUrl = img.ImageUrl,
             })))
             .ReverseMap();
            CreateMap<ReviewImage, ReviewImageDto>();
            CreateMap<Wishlist, WishlistDto>().ReverseMap();
            CreateMap<CreateUpdateWishlistDto, Wishlist>().ReverseMap();
        }
    }
}
