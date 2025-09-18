using System.Text.Json.Serialization;
using eshop_productservice.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eshop_productservice.DataModel;

public class ProductMdb
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [JsonPropertyName("Name")] public required string Name { get; set; }

    public int PriceInCents { get; set; }

    public Product ToModel()
    {
        return new Product
        {
            Id = Id,
            Name = Name,
            PriceInCents = PriceInCents
        };
    }
}