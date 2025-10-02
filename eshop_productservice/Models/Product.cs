namespace eshop_productservice.Models;

public class Product
{
    public string? Id { get; set; }
    
    public string? CategoryId { get; set; }

    public required string Name { get; set; }

    public required int PriceInCents { get; set; }

    public required string ImageUrl { get; set; }

    public required int StarsTimesTen { get; set; }
}