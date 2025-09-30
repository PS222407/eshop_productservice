using eshop_productservice.Data;
using eshop_productservice.DataModel;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eshop_productservice.Repositories;

public class ProductRepositoryMongoDb : IProductRepository
{
    private readonly IMongoCollection<ProductMdb> _productsCollection;

    public ProductRepositoryMongoDb(
        IOptions<DatabaseSettings> eShopDatabaseSettings, ILogger<ProductRepositoryMongoDb> logger)
    {
        var mongoClient = new MongoClient(
            eShopDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eShopDatabaseSettings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<ProductMdb>(
            eShopDatabaseSettings.Value.ProductsCollectionName);

        logger.LogInformation("Products collection created LOGGING");
    }

    public async Task<List<Product>> GetAsync()
    {
        return (await _productsCollection.Find(_ => true).ToListAsync())
            .Select(p => p.ToModel()).ToList();
    }

    public async Task<Product?> GetAsync(string id)
    {
        return (await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync())
            .ToModel();
    }

    public async Task CreateAsync(Product product)
    {
        var productMdb = product.ToProductMdb();
        await _productsCollection.InsertOneAsync(productMdb);
    }

    public async Task UpdateAsync(string id, Product product)
    {
        var productMdb = product.ToProductMdb();
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, productMdb);
    }

    public async Task RemoveAsync(string id)
    {
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
    }

    public Task<PaginationViewModel<Product>> SearchAsync(SearchRequest searchRequest)
    {
        throw new NotImplementedException();
    }
}