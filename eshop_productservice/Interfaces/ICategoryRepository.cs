using eshop_productservice.DTOs;
using eshop_productservice.Models;

namespace eshop_productservice.Interfaces;

public interface ICategoryRepository
{
    public Task<List<CategoryWithProductCountDto>> GetAsync();

    public Task<Category?> GetAsync(string id);
}