using eshop_productservice.Interfaces;
using eshop_productservice.Models;

namespace eshop_productservice.Services;

public class ProductsService(IProductRepository repository)
{
    public async Task<List<Product>> GetAsync()
    {
        return await repository.GetAsync();
    }

    public async Task<Product?> GetAsync(string id)
    {
        return await repository.GetAsync(id);
    }

    public async Task CreateAsync(Product product)
    {
        await repository.CreateAsync(product);
    }

    public async Task UpdateAsync(string id, Product product)
    {
        await repository.UpdateAsync(id, product);
    }

    public async Task RemoveAsync(string id)
    {
        await repository.RemoveAsync(id);
    }
}