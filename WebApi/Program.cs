using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Data; 
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("hShop");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình DbContext với Entity Framework Core
builder.Services.AddDbContext<HshopContext>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MapperProfile));

// Đăng ký các dịch vụ và repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWorkBase, UnitOfWorkBase>(); 
builder.Services.AddScoped<IProductAppService, ProductAppService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
