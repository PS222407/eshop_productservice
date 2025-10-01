using eshop_productservice.ViewModels;

namespace eshop_productservice.Interfaces;

public interface ICategoryService
{
    public Task<List<CategoryViewModel>> Get();

    public Task<CategoryViewModel?> Get(string id);
}