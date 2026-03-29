using System.Text.Json.Serialization;
using LogiTrack.Data;
using Microsoft.EntityFrameworkCore;

internal class Program
{
  private static void Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<LogiTrackContext>(options =>
      options.UseSqlite(builder.Configuration.GetConnectionString("LogiTrackConnection")));

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
  }
}