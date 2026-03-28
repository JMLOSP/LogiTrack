using System.ComponentModel.DataAnnotations;

namespace Logitrack.Models
{
  public class Order
  {
    [Key]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public DateTime DatePlaced { get; set; }

    public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

    public void AddItem(InventoryItem item)
    {
      //validate the item before adding
      if (item == null)
        throw new ArgumentNullException(nameof(item));

      //ensure the item is not already associated with another order
      Items.Add(item);
    }

    public void RemoveItem(int itemId)
    {
      //find the item by id and remove it from the order
      InventoryItem? item = Items.FirstOrDefault(i => i.ItemId == itemId);

      //if the item is found, remove it from the order
      if (item != null)
        Items.Remove(item);
    }

    public string GetOrderSummary()
    {
      //return a summary of the order details
      return string.Format("Order {0} for {1} | Items: {2} | Placer: {3}", OrderId, CustomerName, Items.Count, DatePlaced.ToShortDateString());
    }
  }
}