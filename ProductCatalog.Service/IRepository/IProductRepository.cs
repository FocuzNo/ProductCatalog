﻿using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task EditProduct(Product product);
        Task DeleteProduct(int? id);
    }
}