using eshop_productservice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.Controllers;

[ApiController]
[Route("api/productservice/v1/[controller]")]
public class MigrationController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        await context.Database.MigrateAsync();
        return Ok();
    }
}