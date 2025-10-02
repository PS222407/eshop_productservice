using eshop_productservice.DataModel;
using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.SearchModels;
using eshop_productservice.ViewModels;
using Typesense;
using Typesense.Setup;

namespace eshop_productservice.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    
    private readonly ServiceProvider _provider;
    
    private readonly ITypesenseClient? _typesenseClient;
    
    public ProductService(IProductRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        
        var host = configuration.GetValue<string>("TypeSense:Host") ?? throw new InvalidOperationException();
        var apiKey = configuration.GetValue<string>("TypeSense:ApiKey") ?? throw new InvalidOperationException();
        var protocol = configuration.GetValue<string>("TypeSense:Protocol") ?? throw new InvalidOperationException();
        var port = configuration.GetValue<string>("TypeSense:Port") ?? throw new InvalidOperationException();
        
        _provider = new ServiceCollection()
            .AddTypesenseClient(config =>
            {
                config.ApiKey = apiKey;
                config.Nodes = new List<Node>
                {
                    new(host, port, protocol)
                };
            }, enableHttpCompression: false).BuildServiceProvider();
        _typesenseClient = _provider.GetService<ITypesenseClient>();
    }
    
    public async Task<Product?> GetAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<List<Product>> GetAsync(List<Guid> ids)
    {
        return await _repository.GetAsync(ids);
    }

    public async Task<PaginationDto<Product>> SearchAsync(SearchRequest searchRequest)
    {
        if (_typesenseClient is null) return new PaginationDto<Product>();
        
        var searchParams = new Dictionary<string, string>
        {
            ["q"] = searchRequest.q?.Trim() ?? "",
            ["query_by"] = "Name",
            ["per_page"] = searchRequest.per_page.ToString(),
            ["page"] = searchRequest.page.ToString()
        };
        
        var query = new SearchParameters(searchRequest.q?.Trim() ?? "", "Name");
        var searchResult = await _typesenseClient.Search<ProductSearchModel>("Products", query);

        var result = new PaginationDto<Product>
        {
            found = searchResult.Found,
            hits = searchResult.Hits.Select(p => new Product
            {
                Id = p.Document.Id,
                CategoryId = p.Document.CategoryId,
                Name = p.Document.Name,
                ImageUrl = p.Document.ImgUrl,
                PriceInCents = p.Document.PriceInCents,
                StarsTimesTen = p.Document.StarsTimesTen
            }).ToList(),
            page = searchRequest.page,
            request_params = new RequestParams
            {
                per_page = searchRequest.per_page,
                q = searchRequest.q?.Trim()
            }
        };
        return result;
        
        return await _repository.SearchAsync(searchRequest);
    }

    public async Task CreateCollection()
    {
        if (_typesenseClient is null) return;
        var schema = new Schema(
            "Products",
            new List<Field>
            {
                new("Id", FieldType.String, false),
                new("CategoryId", FieldType.String, true),
                new("Name", FieldType.String, false, null, true),
                new("PriceInCents", FieldType.Int32, false, null, true),
                new("StarsTimesTen", FieldType.Int32, false, null, true),
                new("ImgUrl", FieldType.String),
            });

        var createCollectionResult = await _typesenseClient.CreateCollection(schema);
    }

    public async Task ImportProducts()
    {
        if (_typesenseClient is null) return;
        
        var products = await _repository.Get();

        var productSearchModels = products.Select(p => new ProductSearchModel
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            PriceInCents = p.PriceInCents,
            StarsTimesTen = p.StarsTimesTen,
            ImgUrl = p.ImageUrl
        });
        
        Int32 index = 0;
        var chunked = productSearchModels.Chunk(10000);
        Console.WriteLine(productSearchModels.Count());
        foreach (var chunk in chunked)
        {
            // console write
            Console.WriteLine(index++);
            try
            {
                await _typesenseClient.ImportDocuments("Products", chunk, 40, ImportType.Upsert);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // // sleep 3 seconds
            // Thread.Sleep(1000);
        }
        
        // var createDocumentResult = await _typesenseClient.ImportDocuments("Products", productSearchModels);
        // var a = 1;
    }
}