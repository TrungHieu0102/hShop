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
        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }

}
