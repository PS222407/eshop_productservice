using System.Collections;
using eshop_productservice.Data;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("/api/productservice/v1/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext context) : ControllerBase
{
    [HttpGet("Headers")]
    public IActionResult GetHeaders()
    {
        var headers = Request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        return Ok(headers);
    }

    [HttpGet("Environment")]
    public IActionResult GetEnvironmentVariables()
    {
        var envVars = Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>()
            .ToDictionary(entry => entry.Key.ToString(), entry => entry.Value?.ToString());

        return Ok(envVars);
    }

    [HttpGet("Config")]
    public IActionResult GetConfigValues([FromServices] IConfiguration configuration)
    {
        var configDict = new Dictionary<string, string>();

        foreach (var kvp in configuration.AsEnumerable())
            if (!string.IsNullOrEmpty(kvp.Value))
                configDict[kvp.Key] = kvp.Value;

        return Ok(configDict);
    }

    // [HttpGet("SeedCategories")]
    // public IActionResult SeedCategories()
    // {
    //     const string file = "/home/jens/dev/eshop/eshop_productservice/eshop_productservice/CSV/amazon_categories.csv";
    //
    //     using (var parser = new TextFieldParser(file))
    //     {
    //         parser.TextFieldType = FieldType.Delimited;
    //         parser.SetDelimiters(",");
    //
    //         while (!parser.EndOfData)
    //         {
    //             var row = parser.ReadFields();
    //             if (row[0] == "id") continue;
    //
    //             var category = new Category
    //             {
    //                 // Idd = int.Parse(row[0]),
    //                 Name = row[1]
    //             };
    //             context.Categories.Add(category);
    //             context.SaveChanges();
    //
    //             Console.WriteLine($"id: {row[0]}, category_name: {row[1]}");
    //         }
    //     }
    //
    //     return Ok("hoi");
    // }
    //
    // [HttpGet("SeedProducts")]
    // public IActionResult SeedProducts()
    // {
    //     var categories = context.Categories.ToList();
    //
    //     var products = new List<ProductPdb>();
    //
    //     const string file = "/home/jens/dev/eshop/eshop_productservice/eshop_productservice/CSV/amazon_products.csv";
    //     using (var parser = new TextFieldParser(file))
    //     {
    //         parser.TextFieldType = FieldType.Delimited;
    //         parser.SetDelimiters(",");
    //
    //         while (!parser.EndOfData)
    //         {
    //             var row = parser.ReadFields();
    //             if (row[0] == "asin") continue;
    //
    //             var product = new ProductPdb
    //             {
    //                 // Category = categories.First(c => c.Idd == int.Parse(row[8])),
    //                 Name = row[1],
    //                 ImgUrl = row[2],
    //                 StarsTimesTen = (int)(float.Parse(row[4]) * 10),
    //                 PriceInCents = (int)(float.Parse(row[6]) * 10)
    //             };
    //             products.Add(product);
    //         }
    //     }
    //
    //     context.Products.AddRange(products);
    //     context.SaveChanges();
    //
    //     return Ok("hoi");
    // }
}