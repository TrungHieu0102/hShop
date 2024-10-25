using Bogus;
using Core.Entities;
using Core.Helpers;
using Core.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Emit;


namespace Infrastructure.Data.Context
{
    public class HshopContext : IdentityDbContext<User, Role, Guid>
    {
        public HshopContext(DbContextOptions<HshopContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims")
            .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles")
            .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens")
            .HasKey(x => new { x.UserId });
            builder.Entity<Product>()
            .Property(p => p.Discount)
            .HasColumnType("decimal(18,2)"); 

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            //Seed dữ liệu ngẫu nhiên cho Categories
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Id, f => Guid.NewGuid()) // Tạo Guid ngẫu nhiên
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Slug, (f, c) => SlugHelper.GenerateSlug(c.Name))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence(5))
                .RuleFor(c => c.PictureUrl, f => f.Image.PicsumUrl());

            var categories = categoryFaker.Generate(50); // Tạo 10 category
            builder.Entity<Category>().HasData(categories);

            // Seed dữ liệu ngẫu nhiên cho Suppliers
            var supplierFaker = new Faker<Supplier>()
                .RuleFor(s => s.Id, f => Guid.NewGuid()) // Tạo Guid ngẫu nhiên
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.Logo, f => f.Image.PicsumUrl())
                .RuleFor(s => s.PhoneNumber, f => SlugHelper.GenerateVietnamPhoneNumber(f))
                .RuleFor(s => s.Email, f => f.Internet.Email())
                .RuleFor(s => s.Address, f => f.Address.FullAddress())
                .RuleFor(s => s.Description, f => f.Lorem.Sentence(10));

            var suppliers = supplierFaker.Generate(50); // Tạo 10 supplier
            builder.Entity<Supplier>().HasData(suppliers);

            // Seed dữ liệu ngẫu nhiên cho Products
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid()) // Tạo Guid ngẫu nhiên
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Slug, (f, p) => SlugHelper.GenerateSlug(p.Name)) // Tạo slug từ tên sản phẩm
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(p => p.Unit, f => f.PickRandom(new[] { "Piece", "Box", "Kg" }))
                .RuleFor(p => p.Discount, f => f.Random.Decimal(0, 100))
                .RuleFor(p => p.DateCreated, f => f.Date.Past())
                .RuleFor(p => p.ViewCount, f => f.Random.Int(0, 1000))
                // Assign random CategoryId and SupplierId từ dữ liệu có sẵn
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories.Select(c => c.Id)))
                .RuleFor(p => p.SupplierId, f => f.PickRandom(suppliers.Select(s => s.Id)));

            var products = productFaker.Generate(1000); // Tạo 1000 product
            builder.Entity<Product>().HasData(products);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
               .Entries()
               .Where(e => e.State == EntityState.Added);

            foreach (var entityEntry in entries)
            {
                var dateCreatedProp = entityEntry.Entity.GetType().GetProperty(SystemConsts.DateCreatedField);
                if (entityEntry.State == EntityState.Added
                    && dateCreatedProp != null)
                {
                    dateCreatedProp.SetValue(entityEntry.Entity, DateTime.Now);
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
