using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Data;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Authorization;
using Core.ConfigOptions;
using CloudinaryDotNet;
using Application.Services.Cache;
using Core.Model;
using Application.Services.Payment;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("hShop");
// Check if the JWT configuration is valid
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}
// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v2" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
      

// Configure DbContext with Entity Framework Core
builder.Services.AddDbContext<HshopContext>(options =>
    options.UseSqlServer(connectionString));

// Auth
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<HshopContext>()
    .AddDefaultTokenProviders();
builder.Services.AddHttpContextAccessor(); // Để sử dụng IHttpContextAccessor

//Email

builder.Services.AddTransient<IEmailService, EmailService>();
// Register the policy provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MapperProfile));

// Register services
builder.Services.AddScoped<IUnitOfWorkBase, UnitOfWorkBase>();
// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
// Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
//Supplier
builder.Services.AddScoped<ISupplierService, SupplierService>();
// Auth
builder.Services.AddScoped<IAuthService, AuthService>(); // Ensure AuthService has access to Identity services
//User
builder.Services.AddScoped<IUserService, UserService>();
//Cart
builder.Services.AddScoped<ICartService, CartService>();
//Order
builder.Services.AddScoped<IOrderService, OrderService>();
//Payment
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<PaypalConfiguration>();

// Register Authorization Handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<ITokenService, TokenService>();

// JWT token
builder.Services.Configure<JwtConfigOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddGoogle(options =>
{
    IConfigurationSection googleAuthSection =builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
    options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
           builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//Cloudinary 
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(
       cloudinarySettings?.CloudName,
       cloudinarySettings?.ApiKey,
       cloudinarySettings?.ApiSecret
   ); 
Cloudinary cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<IPhotoService, PhotoService>();
// Redis config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
//Paypal
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPal"));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        await SeedData.InitializeAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API V1");
        c.RoutePrefix = string.Empty; 
    });
}
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

