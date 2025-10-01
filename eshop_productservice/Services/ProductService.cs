using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;

namespace eshop_productservice.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<Product?> GetAsync(string id)
    {
        return await repository.GetAsync(id);
    }

    public async Task<List<Product>> GetAsync(List<Guid> ids)
    {
        return await repository.GetAsync(ids);
    }

    public async Task<PaginationDto<Product>> SearchAsync(SearchRequest searchRequest)
    {
        return await repository.SearchAsync(searchRequest);
    }
}