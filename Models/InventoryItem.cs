using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
  public class InventoryItem
  {
    [Key]
    public int ItemId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public int? OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    public string DisplayInfo()
    {
      return string.Format("Item: {0} | Quantity: {1} | Location: {2}", Name, Quantity, Location);
    }
  }
}