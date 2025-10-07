using System.Text.Json;
using System.Text.Json.Nodes;
using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.SearchModels;
using eshop_productservice.ViewModels;
using Typesense;
using Typesense.Setup;

namespace eshop_productservice.Repositories;

public class SearchRepository : ISearchRepository
{
    private const string ProductsCollection = "Products";

    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    private readonly IProductRepository _productRepository;

    private readonly ITypesenseClient? _typesenseClient;

    public SearchRepository(IConfiguration configuration, IProductRepository productProductRepository)
    {
        var host = configuration.GetValue<string>("TypeSense:Host") ?? throw new InvalidOperationException();
        var apiKey = configuration.GetValue<string>("TypeSense:ApiKey") ?? throw new InvalidOperationException();
        var protocol = configuration.GetValue<string>("TypeSense:Protocol") ?? throw new InvalidOperationException();
        var port = configuration.GetValue<string>("TypeSense:Port") ?? throw new InvalidOperationException();

        _httpClient = new HttpClient();
        _baseUrl = $"{protocol}://{host}:{port}";
        _httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", apiKey);
        _productRepository = productProductRepository;


        var provider = new ServiceCollection()
            .AddTypesenseClient(config =>
            {
                config.ApiKey = apiKey;
                config.Nodes = new List<Node>
                {
                    new(host, port, protocol)
                };
            }).BuildServiceProvider();
        _typesenseClient = provider.GetService<ITypesenseClient>();
    }

    public async Task<PaginationDto<Product>> Products(SearchRequest searchRequest)
    {
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
        var url = $"{_baseUrl}/collections/{ProductsCollection}/documents/search?{queryString}";

        var response = await _httpClient.GetAsync(url);
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

    public async Task CreateProductsCollection()
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
                new("ImgUrl", FieldType.String)
            });

        var createCollectionResult = await _typesenseClient.CreateCollection(schema);
    }

    public async Task ImportProducts()
    {
        if (_typesenseClient is null) return;

        var products = await _productRepository.Get();

        var productSearchModels = products.Select(p => new ProductSearchModel
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            PriceInCents = p.PriceInCents,
            StarsTimesTen = p.StarsTimesTen,
            ImgUrl = p.ImageUrl
        });

        var index = 0;
        var chunked = productSearchModels.Chunk(10000);
        Console.WriteLine(productSearchModels.Count());
        foreach (var chunk in chunked)
        {
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
            if (kvp.Value is JsonArray array)
            {
                var values = array.Select(v => $"{v}");
                var filter = $"{kvp.Key}:=[{string.Join(", ", values)}]";
                parts.Add(filter);
            }

        return string.Join(" && ", parts);
    }
}