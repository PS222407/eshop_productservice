using System.Text.Json.Serialization;

namespace eshop_productservice.SearchModels;

public class ProductSearchModel
{
    [JsonPropertyName("Id")] public string Id { get; set; }

    [JsonPropertyName("CategoryId")] public string CategoryId { get; set; }

    [JsonPropertyName("Name")] public string Name { get; set; }

    [JsonPropertyName("PriceInCents")] public int PriceInCents { get; set; }

    [JsonPropertyName("StarsTimesTen")] public int StarsTimesTen { get; set; }

    [JsonPropertyName("ImgUrl")] public string ImgUrl { get; set; }
}