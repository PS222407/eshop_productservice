using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.SearchModels;
using Typesense;
using Typesense.Setup;
using System.Text.Json;
using System.Text.Json.Nodes;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    private readonly ServiceProvider _provider;

    private readonly ITypesenseClient? _typesenseClient;

    private readonly IConfiguration _config;

    public ProductService(IProductRepository repository, IConfiguration configuration)
    {
        _config = configuration;
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
        var host = _config.GetValue<string>("TypeSense:Host") ?? throw new InvalidOperationException();
        var apiKey = _config.GetValue<string>("TypeSense:ApiKey") ?? throw new InvalidOperationException();
        var protocol = _config.GetValue<string>("TypeSense:Protocol") ?? throw new InvalidOperationException();
        var port = _config.GetValue<string>("TypeSense:Port") ?? throw new InvalidOperationException();

        const string collection = "Products";

        var httpClient = new HttpClient();
        var baseUrl = $"{protocol}://{host}:{port}";
        httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", apiKey);

        var filterByJsonString = searchRequest.filter_by;
        var filterBy = filterByJsonString == null ? null : ConvertToTypesenseFilter(filterByJsonString);

        var searchParams = new Dictionary<string, string?>
        {
            ["q"] = searchRequest.q?.Trim() ?? "",
            ["query_by"] = "Name",
            ["filter_by"] = filterBy,
            ["per_page"] = searchRequest.per_page.ToString(),
            ["page"] = searchRequest.page.ToString()
        };

        var queryString = string.Join("&",
            searchParams.Where(s => s.Value != null)
                .Select(s => $"{Uri.EscapeDataString(s.Key)}={Uri.EscapeDataString(s.Value!)}"));
        var url = $"{baseUrl}/collections/{collection}/documents/search?{queryString}";

        var response = await httpClient.GetAsync(url);
        var jsonString = await response.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<SearchResult<ProductSearchModel>>(jsonString)!;

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
        }
    }

    private static string ConvertToTypesenseFilter(string jsonString)
    {
        var node = JsonNode.Parse(jsonString).AsObject();
        var parts = new List<string>();

        foreach (var kvp in node)
        {
            if (kvp.Value is JsonArray array)
            {
                var values = array.Select(v => $"{v}");
                string filter = $"{kvp.Key}:=[{string.Join(", ", values)}]";
                parts.Add(filter);
            }
        }

        return string.Join(" && ", parts);
    }
}