using eshop_productservice.Repositories;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Services;

public class CategoryService(CategoryRepository repository)
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

    public async Task<CategoryViewModel> Get(string id)
    {
        var category = await repository.GetAsync(id);

        return new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name
        };
    }
}