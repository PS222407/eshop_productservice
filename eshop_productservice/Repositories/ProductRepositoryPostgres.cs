using eshop_productservice.Data;
using eshop_productservice.DataModel;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eshop_productservice.Repositories;

public class ProductRepositoryPostgres(AppDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAsync()
    {
        return (await context.Products.ToListAsync())
            .Select(p => p.ToModel()).ToList();
    }

    public async Task<Product?> GetAsync(string id)
    {
        var guid = new Guid(id);
        return (await context.Products.FindAsync(guid))
            ?.ToModel();
    }

    public async Task CreateAsync(Product product)
    {
        var productPdb = product.ToProductPdb();

        context.Products.Add(productPdb);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(string id, Product product)
    {
        var productPdb = product.ToProductPdb();

        context.Entry(productPdb).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(string id)
    {
        var guid = new Guid(id);
        var product = await context.Products.FindAsync(guid);
        if (product == null) return;
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest)
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

        return new PaginationViewModel<Product>
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

    private async Task<int> SearchCountAsync(SearchRequest searchRequest)
    {
        var query = BuildSearchQuery(searchRequest);
        return await query.CountAsync();
    }

    private IQueryable<ProductPdb> BuildSearchQuery(SearchRequest searchRequest)
    {
        dynamic? filters = searchRequest.filter_by == null
            ? null
            : JsonConvert.DeserializeObject(searchRequest.filter_by);

        string? categoryId = filters?.categories[0];

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