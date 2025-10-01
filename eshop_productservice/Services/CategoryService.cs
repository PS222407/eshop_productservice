using eshop_productservice.Interfaces;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Services;

public class CategoryService(ICategoryRepository repository)
{
    public async Task<List<CategoryViewModel>> Get()
    {
        return (await repository.GetAsync())
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Count = c.Count
            }).ToList();
    }

    public async Task<CategoryViewModel?> Get(string id)
    {
        var category = await repository.GetAsync(id);
        if (category is null) return null;

        return new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name
        };
    }
}