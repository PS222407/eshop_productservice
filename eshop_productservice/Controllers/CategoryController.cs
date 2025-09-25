using eshop_productservice.Services;
using eshop_productservice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class CategoryController(CategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryViewModel>>> Index()
    {
        return await categoryService.Get();
    }
}