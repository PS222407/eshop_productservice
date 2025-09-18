using eshop_productservice.Data;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.repositories;

public class ProductsRepositoryPostgres(AppDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAsync()
    {
        return (await context.Products.ToListAsync())
            .Select(p => p.ToModel()).ToList();
    }

    public async Task<Product?> GetAsync(string id)
    {
        return (await context.Products.FindAsync(id))
            ?.ToModel();
    }

    public async Task CreateAsync(Product product)
    {
        var productPdb = product.ToProductPdb();

        context.Products.Add(productPdb);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(string id, Product product)
    {
        var productPdb = product.ToProductPdb();

        context.Entry(productPdb).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(string id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return;
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}