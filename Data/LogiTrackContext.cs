using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Data
{
  public class LogiTrackContext : DbContext
  {
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Order> Orders => Set<Order>();
  }
}