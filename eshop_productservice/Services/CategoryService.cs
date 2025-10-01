using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;

namespace eshop_productservice.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<List<CategoryWithProductCountDto>> Get()
    {
        return await repository.GetAsync();
    }

    public async Task<Category?> Get(string id)
    {
        return await repository.GetAsync(id);
    }
}