using eshop_productservice.DataModel;

namespace eshop_productservice.Models;

public class Product
{
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required int PriceInCents { get; set; }

    public ProductPdb ToProductPdb()
    {
        var productPdb = new ProductPdb
        {
            Name = Name,
            PriceInCents = PriceInCents
        };

        if (Id != null) productPdb.Id = new Guid(Id);

        return productPdb;
    }

    public ProductMdb ToProductMdb()
    {
        return new ProductMdb
        {
            Id = Id,
            Name = Name,
            PriceInCents = PriceInCents
        };
    }
}