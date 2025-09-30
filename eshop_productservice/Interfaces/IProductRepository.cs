using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>> GetAsync();

    public Task<Product?> GetAsync(string id);
    
    public Task<List<Product>> GetAsync(List<Guid> ids);

    public Task CreateAsync(Product newProductMdb);

    public Task UpdateAsync(string id, Product updatedProductMdb);

    public Task RemoveAsync(string id);

    public Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest);
}