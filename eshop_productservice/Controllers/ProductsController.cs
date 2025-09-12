using eshop_productservice.Models;
using eshop_productservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ProductsService productsService) : ControllerBase
{
    [HttpGet]
    public async Task<List<Product>> Get()
    {
        return await productsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var book = await productsService.GetAsync(id);

        if (book is null) return NotFound();

        return book;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Product newBook)
    {
        await productsService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Product updatedBook)
    {
        var book = await productsService.GetAsync(id);

        if (book is null) return NotFound();

        updatedBook.Id = book.Id;

        await productsService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await productsService.GetAsync(id);

        if (book is null) return NotFound();

        await productsService.RemoveAsync(id);

        return NoContent();
    }
}