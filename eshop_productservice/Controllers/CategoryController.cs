using eshop_productservice.Interfaces;
using eshop_productservice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryViewModel>>> Index()
    {
        return Ok((await categoryService.Get()).Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Count = c.Count
        }).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryViewModel?>> Index(string id)
    {
        var category = await categoryService.Get(id);
        if (category == null)
            return NotFound();

        return Ok(new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name
        });
    }
}