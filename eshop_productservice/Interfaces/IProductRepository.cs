using eshop_productservice.Models;

namespace eshop_productservice.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>> GetAsync();

    public Task<Product?> GetAsync(string id);

    public Task CreateAsync(Product newProductMdb);

    public Task UpdateAsync(string id, Product updatedProductMdb);

    public Task RemoveAsync(string id);
}