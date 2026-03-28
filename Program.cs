using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;

internal class Program
{
  private static void Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<LogiTrackContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("LogiTrackConnection")));

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    using (IServiceScope scope = app.Services.CreateScope())
    {
      LogiTrackContext context = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();

      context.Database.Migrate();

      if (!context.InventoryItems.Any())
      {
        InventoryItem palletJack = new InventoryItem
        {
          Name = "Pallet Jack",
          Quantity = 12,
          Location = "Warehouse A"
        };

        InventoryItem forkliftBattery = new InventoryItem
        {
          Name = "Forklift Battery",
          Quantity = 5,
          Location = "Warehouse B"
        };

        Order order = new Order
        {
          CustomerName = "Samir",
          DatePlaced = new DateTime(2025, 4, 5)
        };

        order.AddItem(palletJack);
        order.AddItem(forkliftBattery);
        order.RemoveItem(999);

        context.Orders.Add(order);
        context.SaveChanges();

        Console.WriteLine(palletJack.DisplayInfo());
        Console.WriteLine(forkliftBattery.DisplayInfo());
        Console.WriteLine(order.GetOrderSummary());
      }
    }

    app.MapGet("/", () => "LogiTrack API running");

    app.Run();
  }
}