//using Application.Interfaces;
//using Application.Mappings;
//using Application.Services.Payment;
//using Application.Services;
//using Core.ConfigOptions;
//using Core.Interfaces;
//using Infrastructure.Data;
//using Infrastructure.UnitOfWork;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WebApi.Authorization;
//using Application.Services.Cache;
//using CloudinaryDotNet;
//using Core.Model;
//using Core.Entities;
//using Infrastructure.Data.Context;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace WebApi.Extensions
//{
//    public static class ServiceRegistrationExtensions
//    {
//        public static void AddSwaggerDocumentation(this IServiceCollection services)
//        {
//            services.AddEndpointsApiExplorer();
//            services.AddSwaggerGen(options =>
//            {
//                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
//                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    In = ParameterLocation.Header,
//                    Description = "Please enter a valid token",
//                    Name = "Authorization",
//                    Type = SecuritySchemeType.Http,
//                    BearerFormat = "JWT",
//                    Scheme = "Bearer"
//                });
//                options.AddSecurityRequirement(new OpenApiSecurityRequirement
//            {
//                {
//                    new OpenApiSecurityScheme
//                    {
//                        Reference = new OpenApiReference
//                        {
//                            Type = ReferenceType.SecurityScheme,
//                            Id = "Bearer"
//                        }
//                    },
//                    new string[] { }
//                }
//            });
//            });
//        }

//        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            var jwtKey = configuration["Jwt:Key"];
//            if (string.IsNullOrEmpty(jwtKey))
//            {
//                throw new InvalidOperationException("JWT Key is not configured.");
//            }

//            services.Configure<JwtConfigOptions>(configuration.GetSection("Jwt"));
//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            .AddGoogle(options =>
//            {
//                IConfigurationSection googleAuthSection = configuration.GetSection("Authentication:Google");
//                options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
//                options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;
//            })
//            .AddJwtBearer(options =>
//            {
//                options.SaveToken = true;
//                options.RequireHttpsMetadata = false;
//                options.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    ValidIssuer = configuration["Jwt:Issuer"],
//                    ValidAudience = configuration["Jwt:Audience"],
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//                };
//            });
//        }

//        public static void AddCorsPolicy(this IServiceCollection services)
//        {
//            services.AddCors(options =>
//            {
//                options.AddPolicy("AllowAllOrigins",
//                       builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//            });
//        }
//        public static void AddDatabaseServices(this IServiceCollection services, string connectionString)
//        {
//            services.AddDbContext<HshopContext>(options =>
//                options.UseSqlServer(connectionString));

//            services.AddIdentity<User, Role>()
//                .AddEntityFrameworkStores<HshopContext>()
//                .AddDefaultTokenProviders();
//        }
//        public static void AddApplicationServices(this IServiceCollection services)
//        {
//            services.AddTransient<IEmailService, EmailService>();
//            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
//            services.AddAutoMapper(typeof(MapperProfile));
//            services.AddScoped<IUnitOfWorkBase, UnitOfWorkBase>();
//            services.AddScoped<IProductRepository, ProductRepository>();
//            services.AddScoped<IProductService, ProductService>();
//            services.AddScoped<ICategoryRepository, CategoryRepository>();
//            services.AddScoped<ICategoryService, CategoryService>();
//            services.AddScoped<ISupplierService, SupplierService>();
//            services.AddScoped<IAuthService, AuthService>();
//            services.AddScoped<IUserService, UserService>();
//            services.AddScoped<ICartService, CartService>();
//            services.AddScoped<IOrderService, OrderService>();
//            services.AddScoped<IPaymentService, PaymentService>();
//            services.AddSingleton<PaypalConfiguration>();
//            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

//            services.AddScoped<ITokenService, TokenService>();

//            // Cloudinary
//            var cloudinarySettings = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("CloudinarySettings").Get<CloudinarySettings>();
//            var account = new Account(
//                cloudinarySettings?.CloudName,
//                cloudinarySettings?.ApiKey,
//                cloudinarySettings?.ApiSecret
//            );
//            Cloudinary cloudinary = new Cloudinary(account);
//            services.AddSingleton(cloudinary);
//            services.AddScoped<IPhotoService, PhotoService>();

//            // Redis config
//            services.AddStackExchangeRedisCache(options =>
//            {
//                options.Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("Redis");
//            });
//            services.AddScoped<ICacheService, RedisCacheService>();

//            // PayPal
//            services.Configure<PayPalSettings>(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("PayPal"));
//        }
//    }
//}
