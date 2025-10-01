using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationViewModel<Product>>> Search([FromQuery] SearchRequest searchRequest)
    {
        return Ok(await productService.SearchAsync(searchRequest));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var product = await productService.GetAsync(id);

        if (product is null) return NotFound();

        return Ok(product);
    }

    [HttpGet("Batch")]
    public async Task<ActionResult<List<Product>>> GetBatch([FromQuery] ProductBatchRequest request)
    {
        if (request.Ids.Count == 0)
            return BadRequest("At least one id must be provided.");

        var products = await productService.GetAsync(request.Ids);
        
        return Ok(products);
    }
}