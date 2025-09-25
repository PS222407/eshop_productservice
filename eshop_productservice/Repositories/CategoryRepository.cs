using eshop_productservice.Data;
using eshop_productservice.Projections;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.repositories;

public class CategoryRepository(AppDbContext context)
{
    public async Task<List<CategoryWithProductCount>> GetAsync()
    {
        return await context.Categories.OrderBy(c => c.Name)
            .Select(c => new CategoryWithProductCount
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Count = context.Products.Count(p => p.CategoryId == c.Id)
            })
            .ToListAsync();
    }
}