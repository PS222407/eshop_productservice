using eshop_productservice.Models;

namespace eshop_productservice.DataModel;

public class ProductPdb
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public int PriceInCents { get; set; }

    public Product ToModel()
    {
        return new Product
        {
            Id = Id.ToString(),
            Name = Name,
            PriceInCents = PriceInCents
        };
    }
}