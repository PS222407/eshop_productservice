using EShop.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EShop.Services;

public class ProductsService
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductsService(
        IOptions<EShopDatabaseSettings> eShopDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            eShopDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            eShopDatabaseSettings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<Product>(
            eShopDatabaseSettings.Value.ProductsCollectionName);
    }

    public async Task<List<Product>> GetAsync() =>
        await _productsCollection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product newProduct) =>
        await _productsCollection.InsertOneAsync(newProduct);

    public async Task UpdateAsync(string id, Product updatedProduct) =>
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task RemoveAsync(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}