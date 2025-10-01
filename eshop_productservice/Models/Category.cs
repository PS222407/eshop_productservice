using eshop_productservice.DataModel;

namespace eshop_productservice.Models;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<ProductDataModel>? Products { get; set; } = [];
}