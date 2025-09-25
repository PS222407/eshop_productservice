using eshop_productservice.DataModel;
using eshop_productservice.ViewModels;

namespace eshop_productservice.Models;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<ProductPdb>? Products { get; set; } = [];

    public CategoryViewModel ToViewModel()
    {
        return new CategoryViewModel()
        {
            Id = Id.ToString(),
            Name = Name,
        };
    }
}