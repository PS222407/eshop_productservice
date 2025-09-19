using eshop_productservice.DataModel;

namespace eshop_productservice.Models;

public class Product
{
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required int PriceInCents { get; set; }

    public required string ImageUrl { get; set; }

    public required int StarsTimesTen { get; set; }

    public ProductPdb ToProductPdb()
    {
        return new ProductPdb
        {
            Name = Name,
            PriceInCents = PriceInCents,
            ImgUrl = ImageUrl,
            StarsTimesTen = StarsTimesTen
        };
    }

    public ProductMdb ToProductMdb()
    {
        return new ProductMdb
        {
            Id = Id,
            Name = Name,
            PriceInCents = PriceInCents,
            ImgUrl = ImageUrl,
            StarsTimesTen = StarsTimesTen
        };
    }
}