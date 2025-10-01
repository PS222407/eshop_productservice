using eshop_productservice.DTOs;
using eshop_productservice.Models;
using eshop_productservice.Requests;

namespace eshop_productservice.Interfaces;

public interface IProductRepository
{
    public Task<Product?> GetAsync(string id);

    public Task<List<Product>> GetAsync(List<Guid> ids);

    public Task<PaginationDto<Product>> SearchAsync(SearchRequest searchRequest);
}