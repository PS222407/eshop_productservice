using eshop_productservice.Models;

namespace eshop_productservice.DataModel;

public class ProductDataModel
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; }

    public required string Name { get; set; }

    public int PriceInCents { get; set; }

    public int StarsTimesTen { get; set; }

    public string ImgUrl { get; set; }

    public Product ToModel()
    {
        return new Product
        {
            Id = Id.ToString(),
            Name = Name,
            PriceInCents = PriceInCents,
            StarsTimesTen = StarsTimesTen,
            ImageUrl = ImgUrl
        };
    }
}