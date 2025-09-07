using EShop.Models;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _productsService;

    public ProductsController(ProductsService productsService) =>
        _productsService = productsService;

    [HttpGet]
    public async Task<List<Product>> Get() =>
        await _productsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var book = await _productsService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Product newBook)
    {
        await _productsService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Product updatedBook)
    {
        var book = await _productsService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedBook.Id = book.Id;

        await _productsService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _productsService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _productsService.RemoveAsync(id);

        return NoContent();
    }
}