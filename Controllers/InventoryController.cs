using LogiTrack.Data;
using LogiTrack.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace LogiTrack.Controllers
{
  [ApiController]
  [Route("api/inventory")]
  [Authorize]
  public class InventoryController : ControllerBase
  {
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;
    public const string InventoryCacheKey = "inventory_list";

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
      _context = context;
      _cache = cache;
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
    {
      Stopwatch stopwatch = Stopwatch.StartNew();

      //try to get the inventory list from the cache; if it's not present, retrieve it from the database and store it in the cache for future requests
      if (!_cache.TryGetValue(InventoryCacheKey, out List<InventoryItem>? inventoryItems))
      {
        inventoryItems = await _context.InventoryItems.AsNoTracking().ToListAsync();

        MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

        _cache.Set(InventoryCacheKey, inventoryItems, cacheOptions);
      }

      stopwatch.Stop();

      //add a custom header to the response indicating how long it took to retrieve the inventory list, which can be useful for monitoring and debugging purposes
      Response.Headers["X-Elapsed-Milliseconds"] = stopwatch.ElapsedMilliseconds.ToString();

      return Ok(inventoryItems);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItem item)
    {
      //validate the incoming inventory item data
      if (item == null)
        return BadRequest("Inventory item cannot be null.");

      //add the new inventory item to the database and save changes
      _context.InventoryItems.Add(item);
      await _context.SaveChangesAsync();

      //invalidate the inventory list cache to ensure that subsequent requests will retrieve the updated list from the database
      _cache.Remove(InventoryCacheKey);

      //return the created inventory item with a 201 Created status code
      return CreatedAtAction(nameof(GetInventory), new { id = item.ItemId }, item);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
      //find the inventory item by id and remove it from the database
      InventoryItem? item = await _context.InventoryItems.FindAsync(id);

      //if the item is not found, return a 404 Not Found response
      if (item == null)
        return NotFound($"Inventory item with ID {id} was not found.");

      //remove the item from the database and save changes
      _context.InventoryItems.Remove(item);
      await _context.SaveChangesAsync();

      //invalidate the inventory list cache to ensure that subsequent requests will retrieve the updated list from the database
      _cache.Remove(InventoryCacheKey);

      //return a 204 No Content response to indicate successful deletion
      return NoContent();
    }
  }
}