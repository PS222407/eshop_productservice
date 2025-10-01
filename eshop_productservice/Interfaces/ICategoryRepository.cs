using eshop_productservice.Models;
using eshop_productservice.Projections;

namespace eshop_productservice.Interfaces;

public interface ICategoryRepository
{
    public Task<List<CategoryWithProductCount>> GetAsync();

    public Task<Category?> GetAsync(string id);
}