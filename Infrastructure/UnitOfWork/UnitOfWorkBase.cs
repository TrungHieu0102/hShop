﻿using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;


namespace Infrastructure.UnitOfWork
{
    public class UnitOfWorkBase : IUnitOfWorkBase
    {
        private readonly HshopContext _context;
        public IProductRepository Products { get; set; }
        public ICategoryRepository Categories { get; set; }
        public ISupplierRepository Suppliers { get; set; }
        public IImageRepository Images { get; set; }
        public ICartRepository Carts { get; set; }
        public IOrderRepository Orders { get; set; }
        public IUserRepository Users { get; set; }
        public ITransactionRepository Transactions { get; set; }
        public IReviewRepository Reviews { get; set; }
        public IReviewImageRepository ReviewImages { get; set; }
        public IWishlistRepository Wishlists { get; set; }
        public UnitOfWorkBase(HshopContext context, IMapper mapper)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context, mapper);
            Suppliers = new SupplierRepository(_context, mapper);
            Images = new ImageRepository(_context);
            Carts = new CartRepository(_context);
            Orders = new OrderRepository(_context);
            Users = new UserRepository(_context);
            Transactions = new TransactionRepository(_context);
            Reviews = new ReviewRepository(_context);
            ReviewImages = new ReviewImageRepository(_context);
            Wishlists = new WishlistRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

    }

}
