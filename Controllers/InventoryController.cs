using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
  [ApiController]
  [Route("api/inventory")]
  public class InventoryController : ControllerBase
  {
    private readonly LogiTrackContext _context;

    public InventoryController(LogiTrackContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
    {
      //retrieve all inventory items from the database and return them as a list
      List<InventoryItem> items = await _context.InventoryItems.ToListAsync();

      return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItem item)
    {
      //validate the incoming inventory item data
      if (item == null)
        return BadRequest("Inventory item cannot be null.");

      //add the new inventory item to the database and save changes
      _context.InventoryItems.Add(item);
      await _context.SaveChangesAsync();

      //return the created inventory item with a 201 Created status code
      return CreatedAtAction(nameof(GetInventory), new { id = item.ItemId }, item);
    }

    [HttpDelete("{id}")]
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

      //return a 204 No Content response to indicate successful deletion
      return NoContent();
    }
  }
}