﻿
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces
{
    public interface IUnitOfWorkBase
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ISupplierRepository Suppliers { get; }  
        IImageRepository Images { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IReviewRepository Reviews { get; }
        IReviewImageRepository ReviewImages { get; }
        IWishlistRepository Wishlists { get; }
        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }

}
