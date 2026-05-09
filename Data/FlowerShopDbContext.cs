using Microsoft.EntityFrameworkCore;
using Practice4.Models;

namespace Practice4.Data;

public class FlowerShopDbContext : DbContext
{
    public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options) : base(options)
    {
    }

    public DbSet<Flower> Flowers => Set<Flower>();
}
