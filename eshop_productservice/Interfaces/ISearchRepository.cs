using eshop_productservice.DTOs;
using eshop_productservice.Models;
using eshop_productservice.Requests;

namespace eshop_productservice.Interfaces;

public interface ISearchRepository
{
    public Task<PaginationDto<Product>> Products(SearchRequest searchRequest);
    public Task CreateProductsCollection();
    public Task ImportProducts();
}