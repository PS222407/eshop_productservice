using eshop_productservice.Data;
using eshop_productservice.DataModel;
using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eshop_productservice.Repositories;

public class ProductRepositoryPostgres(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetAsync(string id)
    {
        var guid = new Guid(id);
        return (await context.Products.FindAsync(guid))
            ?.ToModel();
    }

    public async Task<List<Product>> GetAsync(List<Guid> ids)
    {
        return await context.Products
            .Where(p => ids.Contains(p.Id))
            .Select(p => p.ToModel())
            .ToListAsync();
    }

    public async Task<PaginationDto<Product>> SearchAsync(SearchRequest searchRequest)
    {
        var query = BuildSearchQuery(searchRequest);

        var products = (await query
                .OrderBy(p => p.Id)
                .Skip((searchRequest.page - 1) * searchRequest.per_page)
                .Take(searchRequest.per_page)
                .ToListAsync())
            .Select(p => p.ToModel())
            .ToList();

        var found = await SearchCountAsync(searchRequest);

        return new PaginationDto<Product>
        {
            found = found,
            page = searchRequest.page,
            hits = products,
            request_params = new RequestParams
            {
                per_page = searchRequest.per_page,
                q = searchRequest.q?.Trim()
            }
        };
    }

    public async Task<List<Product>> Get()
    {
        return await context.Products.Select(p => p.ToModel()).ToListAsync();
    }

    public async Task DecreaseStockBy(string productId, int amount)
    {
        var product = await context.Products.FindAsync(new Guid(productId));
        if (product == null) return;
        product.Stock -= amount;
        await context.SaveChangesAsync();
    }

    private async Task<int> SearchCountAsync(SearchRequest searchRequest)
    {
        var query = BuildSearchQuery(searchRequest);
        return await query.CountAsync();
    }

    private IQueryable<ProductDataModel> BuildSearchQuery(SearchRequest searchRequest)
    {
        dynamic? filters = searchRequest.filter_by == null
            ? null
            : JsonConvert.DeserializeObject(searchRequest.filter_by);

        string? categoryId = filters?.CategoryId[0];

        var query = context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchRequest.q))
        {
            var searchTerm = searchRequest.q.Trim().ToLower();
            query = query.Where(p => EF.Functions.ToTsVector("english", p.Name)
                .Matches(EF.Functions.PlainToTsQuery("english", searchTerm)));

            // query = query.Where(p => p.Name.ToLower().Contains(searchRequest.q.ToLower().Trim()));
        }

        if (categoryId != null)
            query = query.Where(p => p.CategoryId == new Guid(categoryId));

        return query;
    }
}