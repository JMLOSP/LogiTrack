using LogiTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Data
{
  public class LogiTrackContext : IdentityDbContext<ApplicationUser>
  {
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options)
      : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Order>()
        .HasMany(order => order.Items)
        .WithOne(item => item.Order)
        .HasForeignKey(item => item.OrderId)
        .OnDelete(DeleteBehavior.SetNull);
    }
  }
}