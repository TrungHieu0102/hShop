﻿using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class HshopContext : IdentityDbContext<User>
    {
        public HshopContext(DbContextOptions<HshopContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
