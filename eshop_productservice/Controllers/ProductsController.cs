using eshop_productservice.Models;
using eshop_productservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class ProductsController(ProductsService productService) : ControllerBase
{
    [HttpGet]
    public async Task<List<Product>> Get()
    {
        return await productService.GetAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var product = await productService.GetAsync(id);

        if (product is null) return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Product product)
    {
        await productService.CreateAsync(product);

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Product updatedProduct)
    {
        var product = await productService.GetAsync(id);

        if (product is null) return NotFound();

        updatedProduct.Id = product.Id;

        await productService.UpdateAsync(id, updatedProduct);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await productService.GetAsync(id);

        if (product is null) return NotFound();

        await productService.RemoveAsync(id);

        return NoContent();
    }
}