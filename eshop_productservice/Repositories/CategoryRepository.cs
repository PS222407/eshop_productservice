using eshop_productservice.Data;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Projections;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<List<CategoryWithProductCountProjection>> GetAsync()
    {
        return await context.Categories.OrderBy(c => c.Name)
            .Select(c => new CategoryWithProductCountProjection
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Count = context.Products.Count(p => p.CategoryId == c.Id)
            })
            .ToListAsync();
    }

    public async Task<Category?> GetAsync(string id)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id.ToString() == id);
    }
}