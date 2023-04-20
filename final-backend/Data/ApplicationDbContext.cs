using Microsoft.EntityFrameworkCore;
using final_backend.Models;

namespace final_backend.Data
{
  public class ApplicationDbContext : DbContext
  {
    public DbSet<Product>? Products { get; set; }
    public DbSet<Order>? Orders { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Product>().HasKey(a => a.Id);
      modelBuilder.Entity<Order>().HasKey(o => o.Id);
      modelBuilder.Entity<Order>().HasOne(o => o.Product).WithMany(p => p.Orders).HasForeignKey(o => o.ProductId);
    }
  }
}