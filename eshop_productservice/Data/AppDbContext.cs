using eshop_productservice.DataModel;
using Microsoft.EntityFrameworkCore;

namespace eshop_productservice.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductPdb> Products { get; set; }
}