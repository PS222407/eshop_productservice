using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using Typesense.Setup;

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
        var provider = new ServiceCollection()
            .AddTypesenseClient(config =>
            {
                config.ApiKey = "mysecretapikey";
                config.Nodes = new List<Node>
                {
                    new Node("0d6auj5nts1ri4xgp-1.a1.typesense.net", "443", "https")
                };
            }, enableHttpCompression: false).BuildServiceProvider();
        
        
        return await repository.SearchAsync(searchRequest);
    }
}