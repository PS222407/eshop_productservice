using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Interfaces;

public interface IProductService
{
    public Task<Product?> GetAsync(string id);

    public Task<List<Product>> GetAsync(List<Guid> ids);

    public Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest);
}