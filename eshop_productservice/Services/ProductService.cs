using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Services;

public class ProductService(IProductRepository repository)
{
    public async Task<Product?> GetAsync(string id)
    {
        return await repository.GetAsync(id);
    }
    
    public async Task<List<Product>> GetAsync(List<Guid> ids)
    {
        return await repository.GetAsync(ids);
    }

    public async Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest)
    {
        return await repository.SearchAsync(searchRequest);
    }
}