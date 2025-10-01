using eshop_productservice.DataModel;
using eshop_productservice.Models;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductDataModel> Products { get; set; }

    public DbSet<Category> Categories { get; set; }
}