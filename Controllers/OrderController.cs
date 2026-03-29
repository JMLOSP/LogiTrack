using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
  [ApiController]
  [Route("api/orders")]
  [Authorize]
  public class OrderController : ControllerBase
  {
    private readonly LogiTrackContext _context;

    public OrderController(LogiTrackContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
      //retrieve all orders from the database, including their associated inventory items, and return them as a list
      List<Order> orders = await _context.Orders.Include(order => order.Items).ToListAsync();

      return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
      //retrieve a specific order by its ID, including its associated inventory items, and return it
      Order? order = await _context.Orders.Include(currentOrder => currentOrder.Items).FirstOrDefaultAsync(currentOrder => currentOrder.OrderId == id);

      //if the order is not found, return a 404 Not Found response
      if (order == null)
        return NotFound($"Order with ID {id} was not found.");

      return Ok(order);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
    {
      //validate the incoming order data and create a new order in the database
      if (request == null)
        return BadRequest("Request cannot be null.");

      //create a new order object and populate it with the provided data
      Order order = new Order
      {
        CustomerName = request.CustomerName,
        DatePlaced = request.DatePlaced
      };

      //if item IDs are provided, validate them and associate the corresponding inventory items with the new order
      if (request.ItemIds != null && request.ItemIds.Count > 0)
      {
        //retrieve the inventory items corresponding to the provided item IDs from the database
        List<InventoryItem> items = await _context.InventoryItems.Where(item => request.ItemIds.Contains(item.ItemId)).ToListAsync();

        //if any of the provided item IDs are invalid (i.e., do not correspond to existing inventory items), return a 400 Bad Request response
        if (items.Count != request.ItemIds.Count)
          return BadRequest("One or more inventory item IDs are invalid.");

        //associate the retrieved inventory items with the new order, ensuring that they are not already associated with another order
        foreach (InventoryItem item in items)
        {
          if (item.OrderId != null)
            return BadRequest($"Inventory item with ID {item.ItemId} is already assigned to another order.");

          order.AddItem(item);
        }
      }

      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
      //find the order by id, including its associated inventory items, and remove it from the database
      Order? order = await _context.Orders.Include(currentOrder => currentOrder.Items).FirstOrDefaultAsync(currentOrder => currentOrder.OrderId == id);

      if (order == null)
        return NotFound($"Order with ID {id} was not found.");

      foreach (InventoryItem item in order.Items)
        item.OrderId = null;

      _context.Orders.Remove(order);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}