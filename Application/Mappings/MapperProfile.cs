using Application.DTOs.AuthsDto;
using Application.DTOs.CategoriesDto;
using Application.DTOs.ProductsDto;
using Application.DTOs.RolesDto;
using Application.DTOs.SuppliersDto;
using AutoMapper;
using Core.Entities;


namespace Application.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();    
            CreateMap<CreateUpdateProductDto, Product>().ReverseMap();  
            CreateMap<CreateUpdateCategoryDto, Category>().ReverseMap();
            CreateMap<SignInDto, User>().ReverseMap();
            CreateMap<RoleDto,Role>().ReverseMap(); 
        }
    }
}
