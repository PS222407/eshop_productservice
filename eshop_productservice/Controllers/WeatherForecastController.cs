using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("/api/productservice/v1/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        logger.LogInformation("Get Weather Forecast LOGGING");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

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

    [HttpGet("SeedProducts")]
    public IActionResult SeedProducts()
    {
        return Ok("hoi");
    }
}