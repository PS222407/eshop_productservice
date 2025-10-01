using eshop_productservice.DTOs;
using eshop_productservice.Models;

namespace eshop_productservice.Interfaces;

public interface ICategoryService
{
    public Task<List<CategoryWithProductCountDto>> Get();

    public Task<Category?> Get(string id);
}