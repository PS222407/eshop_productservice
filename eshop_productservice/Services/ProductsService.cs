using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;

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

    public async Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest)
    {
        return await repository.SearchAsync(searchRequest);
    }
}